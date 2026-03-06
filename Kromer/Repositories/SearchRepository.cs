using Kromer.Data;
using Microsoft.EntityFrameworkCore;

namespace Kromer.Repositories;

public class SearchRepository(KromerContext context)
{
    public async Task<int> CountInvolvedAddress(string address)
    {
        return await context.Transactions.CountAsync(q => q.From == address || q.To == address);
    }

    public async Task<int> CountInvolvedName(string name)
    {
        return await context.Transactions.CountAsync(q => q.Name == name || q.SentName == name);
    }
    
    public async Task<int> CountInvolvedMetadata(string pattern) {
        return await context.Transactions.CountAsync(q => q.Metadata != null && q.Metadata.Contains(pattern));
    }
}