using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    //public record BookDtoForUpdate //Record immutable nesneler için kullanılır
    //                               // Http Get ile request body den alınacak bu veri ve kaynak olarak profile gidecek.
    //                               //Hedef ise Book nesnesi olucak
    //{
    //    public int Id { get; init; } //init ifadesi readonly hale getirir immutable
    //    public String Title { get; init; }
    //    public decimal Price { get; init; }
    //}
    public record BookDtoForUpdate : BookDtoForManipulation
    {
        [Required]
        public int Id { get; init; }
    };
}
