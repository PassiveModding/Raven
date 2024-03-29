using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RavenBOT.Common;
using RavenBOT.Modules.AutoMod.Methods;
using RavenBOT.Modules.AutoMod.Models.Moderation;

namespace RavenBOT.Modules.AutoMod.Modules
{
    [Group("automod")]
    [RavenRequireUserPermission(Discord.GuildPermission.Administrator)]
    [RavenRequireContext(ContextType.Guild)]
    [Summary("Handles automated moderation for servers")]
    public partial class Moderation : ReactiveBase
    {
        public ModerationService ModerationService { get; }
        public HelpService HelpService { get; }

        public Moderation(ModerationService moderationService, HelpService helpService)
        {
            ModerationService = moderationService;
            HelpService = helpService;
        }

        [Command("Help")]
        public async Task HelpAsync()
        {
            var res = await HelpService.PagedHelpAsync(Context, true, new List<string>
            {
                "automod"
            }, "This module handles automated moderation for spam, blacklisted words, tocix chat, ip addresses, explicit usernames, invites, mass mentions and more");

            if (res != null)
            {
                await PagedReplyAsync(res.ToCallBack().WithDefaultPagerCallbacks());
            }
            else
            {
                await ReplyAsync("N/A");
            }
        }

        [Command("AutomodExempt")]
        [Summary("Adds or removed the specified role from the auto-mod exempt list")]
        public async Task AutomodExempt(IRole role)
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            if (config.AutoModExempt.Contains(role.Id))
            {
                config.AutoModExempt.Remove(role.Id);
                await ReplyAsync($"Role removed from auto-mod exempt list.");
            }
            else
            {
                config.AutoModExempt.Add(role.Id);
                await ReplyAsync($"Role added to auto-mod exempt list.");
            }
            ModerationService.SaveModerationConfig(config);
        }

        [Command("UseToxicity")]
        [Summary("Toggles the use of toxic message checking")]
        public async Task ToggleToxicityAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.UsePerspective = !config.UsePerspective;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync($"Toxic message checking: {config.UsePerspective}");
        }

        [Command("SetMaxToxicity")]
        [Summary("Sets the max toxicity for messages (50-100)")]
        public async Task SetMaxToxicityAsync(int max)
        {
            if (max > 50 && max < 100)
            {
                var config = ModerationService.GetModerationConfig(Context.Guild.Id);
                config.PerspectiveMax = max;
                ModerationService.SaveModerationConfig(config);
                await ReplyAsync($"Toxic Message Max Percentage: {config.PerspectiveMax}");
            }
            else
            {
                await ReplyAsync("Max value must be between 50 and 100");
            }
        }

        [Command("BlockMassMentions")]
        [Summary("Toggles blocking of messages where a large amount of unique users or roles are mentioned")]
        public async Task ToggleMentionsAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlockMassMentions = !config.BlockMassMentions;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Mass Mention Config: \n" +
                $"Block Mass Mentions: {config.BlockMassMentions}\n" +
                $"Channels Count: {config.MassMentionsIncludeChannels}\n" +
                $"Users Count: {config.MassMentionsIncludeUsers}\n" +
                $"Roles Count: {config.MassMentionsIncludeRoles}");
        }

        [Command("MassMentionRoles")]
        [Summary("Toggles whether role mentions count towards mass mentions")]
        public async Task ToggleMentionRolesAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.MassMentionsIncludeRoles = !config.MassMentionsIncludeRoles;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Mass Mention Config: \n" +
                $"Block Mass Mentions: {config.BlockMassMentions}\n" +
                $"Channels Count: {config.MassMentionsIncludeChannels}\n" +
                $"Users Count: {config.MassMentionsIncludeUsers}\n" +
                $"Roles Count: {config.MassMentionsIncludeRoles}");
        }

        [Command("MassMentionUsers")]
        [Summary("Toggles whether user mentions count towards mass mentions")]
        public async Task ToggleMentionUsersAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.MassMentionsIncludeUsers = !config.MassMentionsIncludeUsers;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Mass Mention Config: \n" +
                $"Block Mass Mentions: {config.BlockMassMentions}\n" +
                $"Channels Count: {config.MassMentionsIncludeChannels}\n" +
                $"Users Count: {config.MassMentionsIncludeUsers}\n" +
                $"Roles Count: {config.MassMentionsIncludeRoles}");
        }

        [Command("MassMentionChannels")]
        [Summary("Toggles whether channel mentions count towards mass mentions")]
        public async Task ToggleMentionChannelsAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.MassMentionsIncludeChannels = !config.MassMentionsIncludeChannels;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Mass Mention Config: \n" +
                $"Block Mass Mentions: {config.BlockMassMentions}\n" +
                $"Channels Count: {config.MassMentionsIncludeChannels}\n" +
                $"Users Count: {config.MassMentionsIncludeUsers}\n" +
                $"Roles Count: {config.MassMentionsIncludeRoles}");
        }

        [Command("BlockInvites")]
        [Summary("Toggles whether invites will be auto-deleted from chat")]
        public async Task ToggleInvitesAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlockInvites = !config.BlockInvites;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync($"Invite Blocking: {config.BlockInvites}");
        }

        [Command("BlockIps")]
        [Summary("Toggles whether ip addresses will be auto-deleted from chat")]
        public async Task ToggleIpsAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlockIps = !config.BlockIps;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync($"IP Address Blocking: {config.BlockIps}");
        }

        [Command("UseBlacklist")]
        [Summary("Toggles the use of message blacklisting")]
        public async Task ToggleBlacklistAsync()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.UseBlacklist = !config.UseBlacklist;
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync($"Using Blacklist: {config.UseBlacklist}");
        }

        [Command("Blacklist Add")]
        [Summary("Adds a word or message to the blacklist")]
        public async Task BlacklistAdd([Remainder] string message)
        {
            if (message.Length < 3)
            {
                await ReplyAsync("It's not a good idea to add such short items to the blacklist.");
                return;
            }

            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlacklistSimple.Add(new BlacklistSet.BlacklistMessage
            {
                Content = message,
                    Regex = false
            });

            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Added.");
        }

        [Command("Blacklist Remove")]
        [Summary("Removes a message or regex from the blacklist")]
        public async Task BlacklistRemove([Remainder] string message)
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlacklistSimple = config.BlacklistSimple.Where(x => x.Content.Equals(message, StringComparison.InvariantCultureIgnoreCase)).ToList();

            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Removed.");
        }

        [Command("Blacklist Regex Add")]
        [Summary("Adds a regex check to the blacklist")]
        public async Task BlacklistRegexAdd([Remainder] string message)
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlacklistSimple.Add(new BlacklistSet.BlacklistMessage
            {
                Content = message,
                    Regex = true
            });

            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Added.");
        }

        [Command("Blacklist Username Remove")]
        [Summary("Removes a message from the username blacklist")]
        public async Task BlacklistUsernameRemove([Remainder] string name)
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlacklistedUsernames = config.BlacklistedUsernames.Where(x => x.Content.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync($"Removed.");
        }

        [Command("Blacklist Username Add")]
        [Summary("Adds a username to the blacklist")]
        public async Task BlacklistUsernameAdd([Remainder] string name)
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlacklistedUsernames.Add(new BlacklistSet.BlacklistMessage
            {
                Content = name,
                    Regex = false
            });
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync($"Added.");
        }

        [Command("Blacklist Username Regex Add")]
        [Summary("Adds a regex check to the username blacklist")]
        public async Task BlacklistUsernameRegexAdd([Remainder] string name)
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            config.BlacklistedUsernames.Add(new BlacklistSet.BlacklistMessage
            {
                Content = name,
                    Regex = true
            });
            ModerationService.SaveModerationConfig(config);
            await ReplyAsync("Added.");
        }

        [Command("SetToxicityToken")]
        [RavenRequireOwner]
        [Summary("DEV: Sets the perspective.api token")]
        public Task ToxicityToken(string token = null)
        {
            var setup = ModerationService.GetSetup();
            setup.PerspectiveToken = token;
            ModerationService.SetPerspectiveSetup(setup);
            return Task.CompletedTask;
        }

        [Command("ShowSettings")]
        [Summary("Shows the auto-mod settings")]
        public async Task ShowSettings()
        {
            var config = ModerationService.GetModerationConfig(Context.Guild.Id);
            var blacklistSimpleSet = config.BlacklistSimple.Select(x =>
            {
                if (x.Regex)
                {
                    return $"Regex Check: {x.Content}";
                }

                return x.Content;
            }).ToList();
            var exemptlist = config.AutoModExempt.Select(x => Context.Guild.Roles.FirstOrDefault(r => r.Id == x)).Where(x => x != null).Select(x => x.Mention);

            await ReplyAsync("**AUTO-MOD SETTINGS**\n" +
                $"Block Invites: {config.BlockInvites}\n" +
                $"Block IP Addresses: {config.BlockIps}\n" +
                $"Block Mass Mentions: {config.BlockMassMentions}\n" +
                $"Maximum Mentions: {config.MaxMentions}\n" +
                $"Mass Mentions includes Users: {config.MassMentionsIncludeUsers}\n" +
                $"Mass Mentions includes Roles: {config.MassMentionsIncludeRoles}\n" +
                $"Mass Mentions includes Channels: {config.MassMentionsIncludeChannels}\n" +
                $"**TOXICITY SETTINGS (PERSPECTIVE)**\n" +
                $"Use Perspective: {config.UsePerspective}\n" +
                $"Max Toxicity Percent: {config.PerspectiveMax}%\n" +
                $"**ANTI-SPAM SETTINGS**\n" +
                $"Use Anti-Spam: {config.UseAntiSpam}\n" +
                $"Message Cache Size: {config.SpamSettings.CacheSize}\n" +
                $"Max Messages per x Second(s): {config.SpamSettings.MessagesPerTime} messages per {config.SpamSettings.SecondsToCheck} second(s)\n" +
                $"Max Identical Messages: {config.SpamSettings.MaxRepititions}\n" +
                $"**BLACKLIST SETTINGS**\n" +
                $"Use Blacklist: {config.UseBlacklist}\n" +
                $"Blacklisted Words: \n{string.Join("\n", blacklistSimpleSet)}\n" +
                $"\n*Complex Blacklist is currently disabled for ALL Servers as it is still being developed.*\n" +
                $"**BLACKLIST USERNAMES**\n" +
                $"Blacklist Usernames: {config.BlacklistUsernames}\n" +
                $"Blacklisted Usernames: \n{string.Join("\n", config.BlacklistedUsernames)}\n" +
                $"**AUTOMOD EXEMPT**\n" +
                $"All Admin Roles\n" +
                $"Automod Exempt Roles: \n" +
                $"{string.Join("\n", exemptlist)}".FixLength(2047));
        }
    }
}