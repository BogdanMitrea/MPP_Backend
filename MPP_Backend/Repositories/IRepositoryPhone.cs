using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Reflection;

namespace MPP_BackEnd.Repositories
{
    public interface IRepositoryPhone
    {
        public int AddPhone(PhoneModel phoneModel);

        public bool DeletePhone(int id);

        public IEnumerable<PhoneModel> GetAllPhones();

        public PhoneModel? GetPhone(int id);

        public bool UpdatePhone(int id, PhoneModel phone);
    }
}

