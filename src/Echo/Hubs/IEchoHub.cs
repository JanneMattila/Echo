using Echo.Models;
using System.Threading.Tasks;

namespace Echo.Hubs
{
    public interface IEchoHub
    {
        Task Echo(EchoModel echoModel);
    }
}