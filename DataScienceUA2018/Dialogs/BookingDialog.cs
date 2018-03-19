using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DataScienceUA2018.Dialogs
{
    [Serializable]
    public class BookingDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Как на счёт уикенда во Львове на этих выходных?");
            var message = context.MakeMessage();
            var attachment = GetHeroCard();
            message.Attachments.Add(attachment);
            await context.PostAsync(message);
            PromptDialog.Choice(
                context: context,
                resume: ChoiceReceivedAsync,
                options: new List<string> { "Да", "Нет" },
                prompt: "Отель \"Таурус\"",
                retry: "Выберите один из ответов :)",
                promptStyle: PromptStyle.Auto
            );

        }

        private static Attachment GetHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Отель Таурус",
                Subtitle = "24-25 марта 2018",
                Text = "Таурус – новий готельний комплекс у Львові, відкритий в 2014 р. Сучасний, стильний, гостинний, затишний, мабуть один з кращих готелей міста. Розташований в тихому районі, поруч з історичним центром Львова, на відстані лише 1,7 км. від центрального залізничного вокзалу та 1,2 км. від перлини архітектурної спадщини Львова 19-го сторіччя – Національного театру опери та балету.",
                Images = new List<CardImage> { new CardImage("https://q.bstatic.com/images/hotel/max1024x768/778/77860606.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Сайт отеля", value: "http://hotel-taurus.com/") }
            };

            return heroCard.ToAttachment();
        }

  
        private async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            var res = await result;
            if (res.Equals("Да"))
                await context.PostAsync("Супер! Наш менеджер свяжется с вами в ближайшее время. ");
            else
                await context.PostAsync("Подберу для вас что-нибудь другое. ");
            context.Done(Task.CompletedTask);
        }
    }
}