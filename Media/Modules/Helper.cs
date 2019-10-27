using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using RavenBOT.Common;

namespace RavenBOT.Modules.Media.Modules
{
    [Group("media")]
    public class Helper : ReactiveBase
    {
        private HelpService HelpService { get; }

        public Helper(HelpService helpService)
        {
            HelpService = helpService;
        }

        [Command("Help")]
        public async Task HelpAsync()
        {
            var res = await HelpService.PagedHelpAsync(Context, true, new List<string>
            {
                "media"
            });

            if (res != null)
            {
                await PagedReplyAsync(res.ToCallBack().WithDefaultPagerCallbacks());
            }
            else
            {
                await ReplyAsync("N/A");
            }
        }
    }
}