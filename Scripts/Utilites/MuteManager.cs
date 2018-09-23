using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Informatics.Scripts.Utilites {
	public class MuteManager {
		private readonly DiscordSocketClient _bot;
		private DataSet _dataBase;
		private static string Path => $@"{Environment.CurrentDirectory}\Logs\users.xml";

		public MuteManager(DiscordSocketClient bot) {
			_bot = bot;
			UpdateDataBase();
		}

		public void Save(SocketGuildUser user, float duration) {
			var dataRow = _dataBase.Tables[0].NewRow();
			dataRow[0] = user.Id;
			dataRow[1] = user.Guild.Id;
			dataRow[2] = DateTime.Now;
			dataRow[3] = duration;
			dataRow[4] = string.Join(',', user.Roles.Where(role => !role.IsEveryone).Select(role => role.Id));

			_dataBase.Tables[0].Rows.Add(dataRow);

			UpdateDataBase();
		}

		private void UpdateDataBase() {
			if (!File.Exists(Path)) File.Create(Path);
			
			_dataBase?.AcceptChanges();
			_dataBase?.WriteXml(Path, XmlWriteMode.WriteSchema);
			
			_dataBase = new DataSet();
			_dataBase.ReadXml(Path, XmlReadMode.Auto);
		}

		public (IEnumerable<SocketRole>, float) Pop(SocketGuildUser user) {
			var data = _dataBase.Tables[0].Select($"id={user.Id}")[0];

			if (data == null) throw new NullReferenceException("Не найдено подходящей записи в журнале");

			var guildId = (ulong) data["guild_id"];
			var guild = _bot.GetGuild(guildId);
			var duration = (float) data["duration"];

			var roles = ((string) data["roles"])
			            .Split(',')
			            .Select(ulong.Parse)
			            .Select(guild.GetRole);

			_dataBase.Tables[0].Rows.Remove(data);

			UpdateDataBase();

			return (roles, duration);
		}

		public Dictionary<SocketGuildUser, (bool, float)> Check() {
			var result = new Dictionary<SocketGuildUser, (bool, float)>();

			foreach (DataRow row in _dataBase.Tables[0].Rows) {
				var user = _bot.GetGuild((ulong) row["guild_id"]).GetUser((ulong) row["id"]);
				var oldTime = (DateTime) row["timeShtamp"];
				var duration = (float) row["duration"];
				var isDone = DateTime.Now > oldTime.AddMinutes(duration);

				if (isDone)
					_dataBase.Tables[0].Rows.Remove(row);

				var newDur = (float) (DateTime.Now - oldTime.AddMinutes(duration)).TotalMinutes;
				result.Add(user, (isDone, newDur));
			}

			UpdateDataBase();
			return result;
		}
	}
}