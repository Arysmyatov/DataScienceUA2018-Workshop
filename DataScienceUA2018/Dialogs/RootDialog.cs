using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace DataScienceUA2018.Dialogs
{
    [Serializable]
    [LuisModel("9e7e545a-38a1-4bd1-bb4b-7d4e53244c09", "281c46d523b64c2d924d176ed4f09963")]
    public class RootDialog : LuisDialog<object>
    {
        [LuisIntent("EntertainMe")]
        public async Task EntertainMeIntent(IDialogContext context, LuisResult result)
        {

            await context.PostAsync("Пришлите фото");
            context.Call(new FaceApiDialog(), ResumeAfter);
        }
        
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Привет :)");
        }
        [LuisIntent("Tired")]
        public async Task HotelIntent(IDialogContext context, LuisResult result)
        {
            context.Call(new BookingDialog(), ResumeAfter);
        }
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
          
            await context.Forward(new QnaDialog(), ResumeAfter, context.Activity, CancellationToken.None);
        }
        private async Task ResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}