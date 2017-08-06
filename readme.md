# intro-to-botframework-15-skypecallingbot

This is a simplified version of the IVR bot at https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp/skype-CallingBot.  After understanding the principles in this code I recommend that you take a look at this more robust example that demonstrates IVR in more detail.

To get this code running, you will need to update the web.config appSettings:

1. "MicrosoftAppId" - your Microsoft App ID (from dev.botframework.com)
2. "MicrosoftAppPassword" - your Microsoft App Password (from dev.botframework.com)
3. "Microsoft.Bot.Builder.Calling.CallbackUrl" - url to your calling callback endpoint.  This will be something like "https://yourdomain.azurewebsites.net/api/calling/callback"
4. "MicrosoftSpeechApiKey" - api key to access bing speech (from portal.azure.com)

Once you have updated these settings, publish to your hosting environment of your choice, then enable Skype calling in dev.botframework.com.  To do this, "edit" the Skype channel against your bot, and select the calling tab. Set the following values and save:

1. "Enable calling" - checked
2. "IVR" - checked
3. "Webhook (For calling)" - url to your calling call endpoint.  *note this is subtly different to your callback endpoint*.  This will be something like "https://yourdomain.azurewebsites.net/api/calling/call"

Once you have Saved the changes, you can test.

