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
            }
            else
            {
                Console.WriteLine("Ollama não foi encontrado. Iniciando o processo de instalação...");
                await InstallOllamaAsync();
            }

            await CheckAndPullModelAsync("qwen2.5-coder:7b");
        }

        private static async Task CheckAndPullModelAsync(string modelName)
        {
            Console.WriteLine($"Verificando se o modelo '{modelName}' está instalado...");
            
            try
            {
                var checkProcessInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = $"list",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var checkProcess = Process.Start(checkProcessInfo);
                string output = await checkProcess.StandardOutput.ReadToEndAsync();
                await checkProcess.WaitForExitAsync();

                if (output.Contains(modelName))
                {
                    Console.WriteLine($"O modelo '{modelName}' já está instalado.");
                    return;
                }

                Console.WriteLine($"O modelo '{modelName}' não foi encontrado. Baixando modelo (isso pode demorar dependendo da sua internet)...");
                
                var pullProcessInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = $"pull {modelName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var pullProcess = new Process { StartInfo = pullProcessInfo };
                
                pullProcess.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data); };
                pullProcess.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data); };
                
                pullProcess.Start();
                pullProcess.BeginOutputReadLine();
                pullProcess.BeginErrorReadLine();
                
                await pullProcess.WaitForExitAsync();

                if (pullProcess.ExitCode == 0)
                {
                    Console.WriteLine($"Modelo '{modelName}' baixado com sucesso!");
                }
                else
                {
                    Console.WriteLine($"Falha ao baixar o modelo '{modelName}'. Código de saída: {pullProcess.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar ou baixar o modelo '{modelName}': {ex.Message}");
            }
        }

        public static bool IsOllamaInstalled()
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
                Console.WriteLine("Baixando e instalando o Ollama via script bash...");
                
                string bashScript = "if ! command -v zstd > /dev/null; then echo 'Dependência zstd não encontrada. Tentando instalar...'; if command -v apt-get > /dev/null; then sudo apt-get update && sudo apt-get install -y zstd; elif command -v dnf > /dev/null; then sudo dnf install -y zstd; elif command -v pacman > /dev/null; then sudo pacman -S --noconfirm zstd; fi; fi; curl -fsSL https://ollama.com/install.sh | sh";

                var installProcessInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{bashScript}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var installProcess = new Process { StartInfo = installProcessInfo };
                
                installProcess.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data); };
                installProcess.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"Erro: {e.Data}"); };
                
                installProcess.Start();
                installProcess.BeginOutputReadLine();
                installProcess.BeginErrorReadLine();
                
                await installProcess.WaitForExitAsync();

                if (installProcess.ExitCode == 0)
                {
                    Console.WriteLine("Processo de instalação do Ollama finalizado com sucesso.");
                }
                else
                {
                    Console.WriteLine($"A instalação do Ollama falhou com o código de saída: {installProcess.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro durante a instalação do Ollama: {ex.Message}");
            }
        }
    }
}
