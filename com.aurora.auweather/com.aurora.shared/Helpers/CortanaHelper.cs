using System;
using System.Threading.Tasks;

namespace Com.Aurora.Shared.Helpers
{
    public static class CortanaHelper
    {
        public static async Task EditPhraseListAsync(string setName, string listName, string[] phraseList)
        {
            Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinition commandSetEnUs;

            if (Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue(setName, out commandSetEnUs))
            {
                await commandSetEnUs.SetPhraseListAsync(listName, phraseList);
            }
        }
    }
}
