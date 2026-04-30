using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(int id);
    Task<IEnumerable<Inventory>> GetAllByUserIdAsync(int userId);
    Task AddAsync(Inventory entity);
    Task UpdateAsync(Inventory entity);
    Task DeleteAsync(int id);
}