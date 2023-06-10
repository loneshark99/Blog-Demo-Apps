 namespace SimpleHttpService
 {
    using System.Net;
    using Newtonsoft.Json;
    public class Program
    {
        static HttpListener listener = new HttpListener();
        static int count  = 0;
        public static async Task Main()
        {
            listener.Prefixes.Add("http://*:" + 8080 + "/");
            listener.Start();
            Console.WriteLine("Service Started...");
            try
            {
                while(true)
                {
                    var httpContext = await listener.GetContextAsync();
                    Console.WriteLine("Request Url :: " + httpContext.Request.Url);
                    Interlocked.Increment(ref count);
                    var _ = Task.Run(async () => {
                        try
                        {
                            using (var reader  = new StreamReader(httpContext.Request.InputStream))
                            {
                                var input = await reader.ReadToEndAsync();
                                if (httpContext?.Request?.Url?.PathAndQuery == "/")
                                {
                                    switch (httpContext?.Request?.HttpMethod)
                                    {
                                        case "GET":
                                            httpContext.Response.ContentType = "application/json";
                                            var responseObj = new Output() { RequestCount = count };
                                            var response = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseObj));
                                            await httpContext.Response.OutputStream.WriteAsync(response, 0, response.Length).ConfigureAwait(false);
                                            await httpContext.Response.OutputStream.FlushAsync().ConfigureAwait(false);
                                            Console.WriteLine("Request Completed");
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Request Exception : {ex}");
                        }
                        finally
                        {
                            httpContext.Response.OutputStream.Close();
                            httpContext.Response.Close();
                        }
                    });
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"FATAL :: {ex}");
            }
            finally
            {
                listener?.Stop();
                listener?.Close();
            }
        }
    }

    class Output
    {
        public int RequestCount { get; set; }
    }
 }