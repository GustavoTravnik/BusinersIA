using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinersIA
{
    public static class SetupIA
    {
        public static async Task CheckAndInstallOllamaAsync()
        {
            Console.WriteLine("Verificando se o Ollama está instalado no computador...");
            
            if (IsOllamaInstalled())
            {
                Console.WriteLine("Ollama já está instalado e pronto para uso.");
                return;
            }

            Console.WriteLine("Ollama não foi encontrado. Iniciando o processo de instalação...");
            await InstallOllamaAsync();
        }

        private static bool IsOllamaInstalled()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = "-v",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                process?.WaitForExit();
                return process?.ExitCode == 0;
            }
            catch
            {
                // Se der erro (ex: arquivo não encontrado), significa que não está no PATH
                return false;
            }
        }

        private static async Task InstallOllamaAsync()
        {
            try
            {
                string installerUrl = "https://ollama.com/download/OllamaSetup.exe";
                string tempInstallerPath = Path.Combine(Path.GetTempPath(), "OllamaSetup.exe");

                using (var httpClient = new HttpClient())
                {
                    Console.WriteLine("Baixando o instalador do Ollama...");
                    var response = await httpClient.GetAsync(installerUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;

                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    using var fileStream = new FileStream(tempInstallerPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                    
                    var buffer = new byte[8192];
                    bool isMoreToRead = true;
                    long totalRead = 0;

                    while (isMoreToRead)
                    {
                        int read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                            Console.WriteLine();
                            Console.WriteLine("Download concluído com sucesso.");
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            totalRead += read;
                            if (totalBytes.HasValue)
                            {
                                int percentage = (int)((totalRead * 100) / totalBytes.Value);
                                Console.Write($"\rProgresso do download: {percentage}%");
                            }
                        }
                    }
                }

                Console.WriteLine("Executando o instalador do Ollama...");
                var installProcessInfo = new ProcessStartInfo
                {
                    FileName = tempInstallerPath,
                    // Tenta executar silenciosamente, caso o instalador suporte
                    Arguments = "/S", 
                    UseShellExecute = true
                };

                using var installProcess = Process.Start(installProcessInfo);
                installProcess?.WaitForExit();

                Console.WriteLine("Processo de instalação do Ollama finalizado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro durante a instalação do Ollama: {ex.Message}");
            }
        }
    }
}
