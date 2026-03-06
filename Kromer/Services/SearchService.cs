using Kromer.Data;
using Kromer.Models.Api.Krist.Lookup;
using Kromer.Models.Api.Krist.Transaction;
using Kromer.Models.Dto;
using Kromer.Models.Entities;
using Kromer.Repositories;
using Kromer.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace Kromer.Services;

public class SearchService(
    KromerContext context,
    SearchRepository searchRepository,
    WalletRepository walletRepository,
    TransactionRepository transactionRepository,
    NameRepository nameRepository)
{
    public async Task<KristSearchResult> SearchAsync(string query)
    {
        var parsedQuery = SearchValidation.ParseQuery(query);
        var matches = new KristSearchResult.MatchResult
        {
            ExactAddress = parsedQuery.MatchAddress ? await walletRepository.GetAddressAsync(query) : null,
            ExactName = parsedQuery.MatchName ? await nameRepository.GetNameAsync(parsedQuery.StrippedName) : null,
            ExactTransaction = parsedQuery is { MatchTransaction: true, CleanId: not null }
                ? await transactionRepository.GetTransaction(parsedQuery.CleanId.Value)
                : null,
        };

        return new KristSearchResult
        {
            Ok = true,
            Query = parsedQuery,
            Matches = matches,
        };
    }

    public async Task<KristExtendedSearchResult> SearchExtendedAsync(string query)
    {
        var parsedQuery = SearchValidation.ParseQuery(query);

        var matches = new KristExtendedSearchResult.MatchResult();
        var transactions = new KristExtendedSearchResult.MatchTransactions
        {
            AddressInvolved = parsedQuery.MatchAddress ? await searchRepository.CountInvolvedAddress(query) : null,
            NameInvolved = parsedQuery.MatchName ? await searchRepository.CountInvolvedName(query) : null,
            Metadata = await searchRepository.CountInvolvedMetadata(query),
        };

        matches.Transactions = transactions;
        return new KristExtendedSearchResult
        {
            Ok = true,
            Query = parsedQuery,
            Matches = matches,
        };
    }

    public async Task<KristResultTransactions> SearchExtendedResultAsync(TransactionSearchType transactionType,
        string query, TransactionOrderByParameter orderBy, OrderParameter order, bool includeMined, int limit,
        int offset)
    {
        var parsedQuery = SearchValidation.ParseQuery(query);

        var transactions = context.Transactions.AsQueryable();

        transactions = transactionType switch
        {
            TransactionSearchType.Address when parsedQuery.MatchAddress => transactions.Where(q =>
                q.From == query || q.To == query),
            TransactionSearchType.Name when parsedQuery.MatchName => transactions.Where(q => q.Name == query || q.SentName == query),
            TransactionSearchType.Metadata => transactions.Where(q => q.Metadata != null && q.Metadata.Contains(query)),
            _ => null,
        };

        if (transactions == null)
        {
            return new KristResultTransactions
            {
                Ok = true,
                Total = 0,
                Count = 0,
                Transactions = [],
            };
        }

        if (!includeMined)
        {
            transactions = transactions.Where(q => q.TransactionType != TransactionType.Mined);
        }

        if (order == OrderParameter.Asc)
        {
            transactions = orderBy switch
            {
                TransactionOrderByParameter.Id => transactions.OrderBy(q => q.Id),
                TransactionOrderByParameter.From => transactions.OrderBy(q => q.From),
                TransactionOrderByParameter.To => transactions.OrderBy(q => q.To),
                TransactionOrderByParameter.Value => transactions.OrderBy(q => q.Amount),
                TransactionOrderByParameter.Time => transactions.OrderBy(q => q.Date),
                TransactionOrderByParameter.SentName => transactions.OrderBy(q => q.SentName),
                TransactionOrderByParameter.SentMetaname => transactions.OrderBy(q => q.SentMetaname),
                _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null)
            };
        }
        else if (order == OrderParameter.Desc)
        {
            transactions = orderBy switch
            {
                TransactionOrderByParameter.Id => transactions.OrderByDescending(q => q.Id),
                TransactionOrderByParameter.From => transactions.OrderByDescending(q => q.From),
                TransactionOrderByParameter.To => transactions.OrderByDescending(q => q.To),
                TransactionOrderByParameter.Value => transactions.OrderByDescending(q => q.Amount),
                TransactionOrderByParameter.Time => transactions.OrderByDescending(q => q.Date),
                TransactionOrderByParameter.SentName => transactions.OrderByDescending(q => q.SentName),
                TransactionOrderByParameter.SentMetaname => transactions.OrderByDescending(q => q.SentMetaname),
                _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null)
            };
        }

        var total = await transactions.CountAsync();

        transactions = transactions
            .Skip(offset)
            .Take(limit);

        var entities = await transactions.ToListAsync();

        return new KristResultTransactions
        {
            Ok = true,
            Total = total,
            Count = entities.Count,
            Transactions = entities.Select(TransactionDto.FromEntity).ToList()
        };
    }
}