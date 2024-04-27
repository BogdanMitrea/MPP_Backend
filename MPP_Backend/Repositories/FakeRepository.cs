namespace MPP_BackEnd.Repositories
{
    public class FakeRepository:IRepositoryPhone
    {
        private List<PhoneModel> phones;

        public FakeRepository(List<PhoneModel> phones)
        {
            this.phones = phones;
        }

        public int get_maxID()
        {
            int maxid = 0;
            foreach (var a in phones)
            {
                if (a.Id > maxid)
                {
                    maxid = a.Id;
                }
            }
            return maxid + 1;
        }

        public int AddPhone(PhoneModel phoneModel)
        {
            int newid = get_maxID();
            phoneModel.Id = newid;
            phones.Add(phoneModel);
            return phoneModel.Id;
        }

        public bool DeletePhone(int id)
        {
            int indexToRemove = phones.FindIndex(phone => phone.Id == id);
            if (indexToRemove != -1)
            {
                phones.RemoveAt(indexToRemove);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<PhoneModel> GetAllPhones()
        {
            return phones;
        }

        public PhoneModel? GetPhone(int id)
        {
            return phones.ElementAt(phones.FindIndex(phone => phone.Id == id));
        }

        public bool UpdatePhone(int id, PhoneModel phone)
        {
            int indexToUpdate = phones.FindIndex(phone => phone.Id == id);
            if (indexToUpdate != -1)
            {
                phones[indexToUpdate].Name = phone.Name;
                phones[indexToUpdate].Memory = phone.Memory;
                phones[indexToUpdate].Producer = phone.Producer;
                phones[indexToUpdate].Color = phone.Color;
                phones[indexToUpdate].Photo = phone.Photo;
                phones[indexToUpdate].Year = phone.Year;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
