using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Configuration;
using DataScienceUA2018.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace DataScienceUA2018.Dialogs
{
    [Serializable]
    public class FaceApiDialog : IDialog<object>
    {
        private const string uriBase = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/detect";


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            if (activity.Attachments.Any())
            {
                var attachmentUrl =
                    activity.Attachments[0].ContentUrl;

                var httpClient = new HttpClient();

                var attachmentData =
                    await httpClient.GetByteArrayAsync(attachmentUrl);
                var res = await MakeAnalysisRequest(attachmentData);
               
                string emotion;
                var gender = res.gender.Equals("male") ? "мужчина" : "женщина";

                if (res.emotion.anger > 0.8)
                    emotion = "в злом настроении";
                else if (res.emotion.disgust > 0.8)
                    emotion = "в раздраженном настроении";
                else if (res.emotion.fear > 0.8)
                    emotion = "в злом настроении";
                else if (res.emotion.happiness > 0.8)
                    emotion = "в счастливом настроении. Ураа \U0001F60D ";
                else if (res.emotion.neutral > 0.8)
                    emotion = "в нейтральном настроении. ";
                else if (res.emotion.sadness > 0.8)
                    emotion = "в грустном настроении. Поболтайте со мной и оно улучшится! ";
                else if (res.emotion.surprise > 0.8)
                    emotion = "в удивленном настроении. ";
                else 
                    emotion = ". Пока что не смог уловить ваше настроение. ";
                await context.PostAsync(string.Format("На фото {0} примерно {1} лет {2}", gender, res.age, emotion));
            }
            else
            {
                await context.PostAsync("Эх, а я ждал фото :(");
            }
            context.Done(Task.CompletedTask);
        }
        public async Task<FaceAttributes> MakeAnalysisRequest(byte[] byteData)
        {
            HttpClient client = new HttpClient();
            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", WebConfigurationManager.AppSettings["FaceApiKey"]);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=false&returnFaceLandmarks=false&returnFaceAttributes=age,gender,emotion";

            // Assemble the URI for the REST API Call.
            string uri = uriBase +"?" + requestParameters ;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                var des = JsonConvert.DeserializeObject<List<FaceApiResponse>>(contentString);
                return des.FirstOrDefault().faceAttributes;
            }
        }

    }
}