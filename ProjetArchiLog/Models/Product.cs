using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryArchiLog.Models;


namespace ProjetArchiLog.Models
{
    public class Brand : BaseModel
    {
        //[Key]
        //public int ID { get; set; }
        //public DateTime CreatedAt { get; set; }
        [StringLength(50)]
        [Required()]
        public string? Name { get; set; }

        //[Column("Name=nameColumn")]
        public string? Slogan { get; set; }

        //public List<Car>? Cars { get; set; }
        //public IEnumerable<Car> Cars { get; set; } boucle infini pour l'Api  

    }
}
