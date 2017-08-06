using Microsoft.Bot.Builder.Calling;
using Microsoft.Bot.Builder.Calling.Events;
using Microsoft.Bot.Builder.Calling.ObjectModel.Contracts;
using Microsoft.Bot.Builder.Calling.ObjectModel.Misc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoiceBot.CogServices;

namespace VoiceBot.Voice
{
    public class DemoCallingBot : IDisposable, ICallingBot
    {
        private readonly MicrosoftCognitiveSpeechService speechService = new MicrosoftCognitiveSpeechService();

        public DemoCallingBot(ICallingBotService callingBotService)
        {
            this.CallingBotService = callingBotService;

            this.CallingBotService.OnIncomingCallReceived += this.OnIncomingCallReceived;
            this.CallingBotService.OnRecordCompleted += this.OnRecordCompleted;
            this.CallingBotService.OnHangupCompleted += OnHangupCompleted;
        }

        public ICallingBotService CallingBotService { get; }

        public void Dispose()
        {
            if (this.CallingBotService != null)
            {
                this.CallingBotService.OnIncomingCallReceived -= this.OnIncomingCallReceived;
                this.CallingBotService.OnRecordCompleted -= this.OnRecordCompleted;
                this.CallingBotService.OnHangupCompleted -= OnHangupCompleted;
            }
        }

        private static Task OnHangupCompleted(HangupOutcomeEvent hangupOutcomeEvent)
        {
            hangupOutcomeEvent.ResultingWorkflow = null;
            return Task.FromResult(true);
        }

        private static PlayPrompt GetPromptForText(string text)
        {
            var prompt = new Prompt { Value = text, Voice = VoiceGender.Male };
            return new PlayPrompt { OperationId = Guid.NewGuid().ToString(), Prompts = new List<Prompt> { prompt } };
        }

        private Task OnIncomingCallReceived(IncomingCallEvent incomingCallEvent)
        {
            var record = new Record
            {
                OperationId = Guid.NewGuid().ToString(),
                PlayPrompt = new PlayPrompt { OperationId = Guid.NewGuid().ToString(), Prompts = new List<Prompt> { new Prompt { Value = "Please leave a message" } } },
                RecordingFormat = RecordingFormat.Wav
            };

            incomingCallEvent.ResultingWorkflow.Actions = new List<ActionBase> {
                new Answer { OperationId = Guid.NewGuid().ToString() },
                record
            };

            return Task.FromResult(true);
        }

        private async Task OnRecordCompleted(RecordOutcomeEvent recordOutcomeEvent)
        {
            List<ActionBase> actions = new List<ActionBase>();

            var spokenText = string.Empty;
            if (recordOutcomeEvent.RecordOutcome.Outcome == Outcome.Success)
            {
                var record = await recordOutcomeEvent.RecordedContent;
                spokenText = await this.speechService.GetTextFromAudioAsync(record);
                actions.Add(new PlayPrompt { OperationId = Guid.NewGuid().ToString(), Prompts = new List<Prompt> { new Prompt { Value = "Thanks for leaving the message." }, new Prompt { Value = "You said... " + spokenText } } });
            }
            else
            {
                actions.Add(new PlayPrompt { OperationId = Guid.NewGuid().ToString(), Prompts = new List<Prompt> { new Prompt { Value = "Sorry, there was an issue. " } } });
            }

            actions.Add(new Hangup { OperationId = Guid.NewGuid().ToString() }); // hang up the call

            recordOutcomeEvent.ResultingWorkflow.Actions = actions;
            recordOutcomeEvent.ResultingWorkflow.Links = null;
        }
    }
}