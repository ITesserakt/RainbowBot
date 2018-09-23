using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Informatics.Scripts.Modules {
	public class DeveloperModule : ModuleBase<SocketCommandContext> {
		[Command("role info")]
		[Summary("Для разработки; выводит некоторые полезные параметры роли")]
		public async Task RoleInfo(IRole role) {
			await ReplyAsync(embed: new EmbedBuilder {
				Color = role.Color,
				Fields = new List<EmbedFieldBuilder> {
					new EmbedFieldBuilder {
						Name = "Id",
						Value = role.Id
					},
					new EmbedFieldBuilder {
						Name = "Position",
						IsInline = true,
						Value = role.Position
					},
					new EmbedFieldBuilder {
						Name = "Created",
						IsInline = true,
						Value = role.CreatedAt
					},
					new EmbedFieldBuilder {
						Name = "Permissions",
						IsInline = true,
						Value = string.Join(", ", role.Permissions.ToList())
					}
				}
			}.Build(), message: "");
		}
	}
}