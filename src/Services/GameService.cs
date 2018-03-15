using DEA.Extensions.Database;
using DEA.Database.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DEA.Services.Static;
using MongoDB.Driver;
using DEA.Entities.Service;
using DEA.Entities.Item;

namespace DEA.Services
{
    public sealed class GameService : Service
    {
        private readonly IMongoCollection<User> _userRepo;

        public GameService(IMongoCollection<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IReadOnlyDictionary<string, int>> MassOpenCratesAsync(Crate crate, int quantity, User dbUser = null)
        {
            var itemsToAdd = new Dictionary<string, int>();

            for (int i = 0; i < quantity; i++)
            {
                var item = await OpenCrateAsync(crate);

                if (itemsToAdd.TryGetValue(item.Name, out int itemCount))
                {
                    itemsToAdd[item.Name]++;
                }
                else
                {
                    itemsToAdd.Add(item.Name, 1);
                }
            }

            if (dbUser != null)
            {
                foreach (var item in itemsToAdd)
                {
                    await ModifyInventoryAsync(dbUser, item.Key, item.Value);
                }

                await ModifyInventoryAsync(dbUser, crate.Name, -quantity);
            }

            return itemsToAdd;
        }

        public async Task<Item> OpenCrateAsync(Crate crate, User dbUser = null)
        {
            int cumulative = 0;
            int roll = CryptoRandom.Next(1, Data.CrateItemOdds);

            if (crate.ItemOdds >= CryptoRandom.Roll())
            {
                foreach (var item in Data.CrateItems)
                {
                    cumulative += item.CrateOdds;

                    if (roll < cumulative)
                    {
                        if (dbUser != null)
                        {
                            await ModifyInventoryAsync(dbUser, crate.Name, -1);
                            await ModifyInventoryAsync(dbUser, item.Name);
                        } 
                        return item;
                    }
                }
            }
            else
            {
                if (dbUser != null)
                {
                    await ModifyInventoryAsync(dbUser, crate.Name, -1);
                    await ModifyInventoryAsync(dbUser, "Bullet");
                }
                return Data.Items.First(x => x.Name == "Bullet");
            }
            return null;
        }

        public async Task ModifyInventoryAsync(User DbUser, string item, int amountToAdd = 1)
        {
            if (DbUser.Inventory.Contains(item))
            {
                await _userRepo.UpsertUserAsync(DbUser.UserId, DbUser.GuildId, x => x.Inventory[item] = amountToAdd + x.Inventory[item].AsInt32);
            }
            else
            {
                await _userRepo.UpsertUserAsync(DbUser.UserId, DbUser.GuildId, x => x.Inventory.Add(item, amountToAdd));
            }

            if (DbUser.Inventory[item] <= 0)
            {
                await _userRepo.UpsertUserAsync(DbUser.UserId, DbUser.GuildId, x => x.Inventory.Remove(item));
            }
        }

        public IEnumerable<Item> InventoryData(User dbUser)
        {
            return Data.Items.Where(x => dbUser.Inventory.Names.Any(y => y == x.Name));
        }
    }
}

