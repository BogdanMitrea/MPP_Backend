using System.ComponentModel.DataAnnotations;

namespace MPP_Backend
{
    public class Store
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
