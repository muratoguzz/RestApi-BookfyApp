using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    //AssemblyReference genellikle boş bir sınıftır ve yalnızca derlemeyi işaret etmek için kullanılır.
    public class AssemblyReference
    {
        //AssemblyReference sınıfı, Presentation katmanının assembly'sini belirtmek ve bu assembly'deki controller'ların
        //MVC framework tarafından tanınmasını sağlamak için kullanılıyor. Bu sayede, Presentation katmanındaki
        //controller'lar projeye eklenmiş oluyor.Bu tür bir yapı, uygulamanın modüler olmasına ve farklı katmanların
        //ayrı ayrı yönetilmesine olanak tanır.
    }
}
