using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.nTactServer.Messages

{
    public class FetchRecipesMessage : MessageBase
    {
        #region Constants

        protected const string MsgTypeTag = "RECIPES";
        private const int AverageRecipeSize = 12000;

        #endregion

        #region Member Data

        #endregion

        #region Properties

        public List<RecipeData> Files { get; } = new List<RecipeData>();

        public override string MessageType => MsgTypeTag;

        public override string MessageBody => "";

        public override string MessageReplyBody => MakeReplyString();

        #endregion

        #region Functions

        public FetchRecipesMessage() : base() { }

        public bool WriteRecipesToFolder(string folderName)
        {
            try
            {
                if (!Directory.Exists(folderName))
                {
                    return false;
                }

                Files.ForEach(f => f.WriteFile(folderName));

                return true;
            }
            catch (Exception ex)
            {
                Trace.Listeners["nTact"].WriteLine($"Exception caught trying to write coater recipe files to disk: {ex.Message}");
                return false;
            }
        }

        public override void ParseMessageBody(string messageBody)
        {
            // Nothing to do -- no body on this message
        }

        public override void ParseReplyBody(string replyBody)
        {
            Files.Clear();

            var parts = replyBody.Split(new char[] { Separator }).ToList();

            // Check that the count is a valid number, and that it matches the number of message parts
            if (!int.TryParse(parts[0], out int count) || count != parts.Count - 1)
            {
                return;
            }

            for (int i = 1; i < parts.Count; i++)
            {
                Files.Add(RecipeData.ParseFromFileDataString(parts[i]));
            }
        }

        public void AddFile(RecipeData recipeData)
        {
            Files.Add(recipeData);
        }

        public async Task AddFileAsync(RecipeData recipeData)
        {
            await (Task.Run(() => Files.Add(recipeData)));
        }

        private string MakeReplyString()
        {
            StringBuilder builder = new StringBuilder($"{Files.Count:0000}", AverageRecipeSize * Files.Count);

            foreach (var fd in Files)
            {
                builder.Append($"{Separator}{fd}");
            }

            return builder.ToString();
        }

        #endregion
    }
}
