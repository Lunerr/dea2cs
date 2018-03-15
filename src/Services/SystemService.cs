using Discord.Commands;
using DEA.Common;
using DEA.Entities.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DEA.Services
{
    public sealed class ListService : Service
    {
        public string ListCommands(IEnumerable<CommandInfo> cmds)
            => List(cmds, x => x.Name, x => x.Name.Length, x => x.Summary);

        public string ListModules(IEnumerable<ModuleInfo> modules)
            => List(modules, x => x.Name, x => x.Name.Length, x => x.Summary);

        public string List<T, TSummary>(IEnumerable<T> elements, Func<T, string> nameSelector, Func<T, int> lengthSelector,
            Func<T, TSummary> summarySelector)
        {
            var listBuilder = new StringBuilder("```");
            var padding = nameSelector(elements.OrderByDescending(lengthSelector).First()).Length + 2;

            foreach (var element in elements.OrderBy(nameSelector))
                listBuilder.AppendFormat("{0}{1}{2}\n", Config.PREFIX, nameSelector(element).PadRight(padding), summarySelector(element));

            listBuilder.Append("```");

            return listBuilder.ToString();
        }
    }
}
