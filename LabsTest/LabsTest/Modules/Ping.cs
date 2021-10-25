using System.Threading.Tasks;
using Discord.Commands;

namespace LabsTest.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {

        [Command("ping")]
        public async Task Pong()
        {
            await Context.Channel.SendMessageAsync("Pong");
        }
    }
}