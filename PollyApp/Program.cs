using Amazon.Polly;
using Amazon.Polly.Model;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (AmazonPollyClient client = new AmazonPollyClient())
            {
                while (true)
                {
                    string text = Console.ReadLine();
                    if (!string.IsNullOrEmpty(text))
                    {
                        SynthesizeSpeechRequest request = new SynthesizeSpeechRequest();
                        request.Text = text;
                        request.TextType = TextType.Text;
                        request.VoiceId = VoiceId.Amy;
                        request.OutputFormat = OutputFormat.Mp3;
                        SynthesizeSpeechResponse response = client.SynthesizeSpeech(request);
                        using (MemoryStream memStream=new MemoryStream())
                        {
                            response.AudioStream.CopyTo(memStream);
                            memStream.Position = 0;

                            using (WaveStream blockAlignedStream =new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(memStream))))
                            {
                                using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                                {
                                    waveOut.Init(blockAlignedStream);
                                    waveOut.Play();
                                    while (waveOut.PlaybackState == PlaybackState.Playing)
                                    {
                                        System.Threading.Thread.Sleep(100);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
