using MPP_BackEnd;

namespace MPP_Backend.Repositories
{
    public interface IRepositoryStore
    {
        public int AddStore(Store store);

        public bool DeleteStore(int id);

        public IEnumerable<Store> GetAllStores();

        public Store? GetStore(int id);

        public bool UpdateStore(int id, Store store);
    }
}
