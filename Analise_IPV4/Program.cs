using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Analise_IPV4
{
    internal class Program
    {
        static (String ip, int mask) ipv4 = ("", 0);
        static List<IpAddress> ListaIPs = new List<IpAddress>();
        static void Main(string[] args)
        {
            String input = "";
            ConsoleKeyInfo Key;
            do
            {
                Console.Clear();
                Console.WriteLine("---------------------------Programa de Console para cálculos básicos de IPV4-----------------------\n");
                Console.WriteLine("O padrão CIDR e por extenso da máscara são aceitos.");
                Console.WriteLine("Cada campo deve possuir 3 dígitos, enquanto o campo CIDR (caso utilizado), deve possuir 2 dígitos.");
                Console.WriteLine("Siga os exemplos abaixo: ");
                Console.WriteLine("CIDR:    192.168.000.054/08");
                Console.WriteLine("Extenso: 192.168.000.054 255.000.000.000");
                Console.Write("Informe IP e máscara:");
                input = Console.ReadLine().Trim();

                if (ValidaEntrada(input))
                {
                    if (!ValidaIP(ipv4.ip))
                    {
                        Console.WriteLine("Endereço IP apresenta erro lógico!");
                    }
                    else if (ipv4.mask == 0) Console.WriteLine("Máscara inválida");
                    else
                    {
                        if (ipv4.ip.Equals("000.000.000.000") || ipv4.ip.Equals("255.255.255.255")) Console.WriteLine("Endereço IP inválido!");
                        else
                        {
                            CalculaIP(ipv4);
                        }
                    }
                }
                else Console.WriteLine("A entrada não segue a formatação padrão, digite novamente conforme os exemplos!");

                Console.WriteLine("Pressione qualquer tecla para calcular um novo IP, ou x para sair");
                Console.WriteLine("Se desejar visualizar os IPs já calculados, tecle \"L\"");
                Key = Console.ReadKey();
                if (Key.Key == ConsoleKey.L)
                {
                    Console.Clear();
                    Console.WriteLine("Lista de IPs calculados: \n");
                    foreach (IpAddress ip in ListaIPs)
                    {
                        Console.WriteLine(ip);
                    }
                    Console.WriteLine("Pressione qualquer tecla para calcular um novo IP, ou x para sair");
                    Key = Console.ReadKey();
                }

            } while(Key.Key != ConsoleKey.X);
        }

        //Método ValidaEntrada() deve ser refatorado
        public static bool ValidaEntrada(String input)                //Retorna true se o formato padrão de entrada estiver correto 
        {                                                             //Analisa se entrada está em CIDR ou por extenso   
            int tam = input.Length;                                   //Valida a máscara em ToCidrMask() e caso entrar já em CIDR
            int cidr = 0;

            if (input[tam - 3].Equals('/'))                           //Caso o CIDR for menor/igual que 0 ou igual/maior que 31, cidr = 0; 
            {
                ipv4.ip = input.Substring(0, tam - 3);
                bool sucess = int.TryParse(input.Substring(tam - 2, 2), out cidr);
                if (cidr >= 0 && cidr < 31)
                    ipv4.mask = cidr;
                else ipv4.mask = 0;
            }
            else if (input[tam - 16].Equals(' '))
            {
                ipv4.ip = input.Substring(0, tam - 16);
                ipv4.mask = ToCidrMask(input.Substring(tam - 15));
            }
            else return false;

            return true;
        }

        public static int ToCidrMask(String mask)                     //Transforma a máscara por extenso para CIDR contando a quantidade de bits 1 ligados
        {                                                             //Qtd de bits 1 ligados virá da função ValidaMask()  
            int cidr = 0;                                             //Por retornar inteiros, o retorno 0 indica a falha da operação
            String maskBit = ValidaMask(mask);
            if (!maskBit.Equals("0"))
            {
                if (maskBit.Contains("0"))
                {
                    cidr = maskBit.IndexOf('0');
                }
            }
            if (cidr == 31) cidr = 0;
            return cidr;
        }

        public static String ValidaMask(String mask)    //Primeiramente chama a função ValidaOctetos, caso os octetos não estejam no padrão, retorna mask=0 (falha na operação), 
        {                                               //Se válidos, transforma os octetos(decimal) para ocetetos(binários)
            String[] octetos = mask.Split('.');         //Concatena todos os octetos em 1 String, percorre a String até encontrar o primeiro 0 e divide em 2 Strings
            String maskBit = "";                        //Se houver bit '1' na segunda String, máscara inválida, return 0
                                                        //Senao, sucesso no procedimento, return máscara em binário
            if (ValidaOctetos(octetos))
            {
                String[] octetosBit = new string[4];
                String maskBitRede = "";
                String maskBitHost = "";
                int index = 0;
                foreach (String octeto in octetos)       //Converte octetos de decimal para binário
                {
                    octetosBit[index] = Convert.ToString(int.Parse(octeto), 2);
                    index++;
                }
                foreach (String octetoBit in octetosBit) maskBit += octetoBit; //concatena os resultados dos octetos binários  

                int indexFinalRede = maskBit.IndexOf("0");            //Etapa onde percorre a String até encontrar o primeiro 0
                maskBitRede = maskBit.Substring(0, indexFinalRede);
                maskBitHost = maskBit.Substring(indexFinalRede);

                return (maskBitRede.Contains("0") || maskBitHost.Contains("1")) ? "0" : maskBit; //código abaixo mantido para melhor compreensão
                /*if (maskBitRede.Contains("0") || maskBitHost.Contains("1")) Console.WriteLine("Máscara inválida!!!");
                else return maskBit; // utilizar operador ternário posteriormente
                */
            }
            return maskBit = "0";
        }

        public static bool ValidaIP(String ip)                      //Recebe o ip
        {                                                           //Chama ValidaOctetos()
            // valid = false;
            String[] octetosIP = ip.Split('.');
           
            Console.WriteLine("Analisando endereço IP...");
            return (ValidaOctetos(octetosIP));

            //Console.WriteLine("Endereço IP segue o padrão!");
            
        }

        public static bool ValidaOctetos(String[] Octetos)         //Recebe um vetor de octetos
        {                                                          //Percorre validando o octeto: para ser válido deve ser maior/igual a 0 e menor/igual a 255
            bool valid = true;
            int octInt;
            foreach (String octString in Octetos)
            {
                if (int.TryParse(octString, out octInt))
                {
                    if (octInt < 0 || octInt > 255) valid = false;
                }
                else return valid = false;
            }
            return valid;
        }

        public static void CalculaIP((String ip, int mask) ipv4)             //Recebe a entrada (ip+mask)
        {
            IpAddress novoIP;
            if (ipv4.mask == 8 || ipv4.mask == 16 || ipv4.mask == 24) novoIP = new IpAddress(ipv4, false); //Senao, chama CalculaIpSubRede()
            else novoIP = new IpAddress(ipv4, true); //CalculaIp(ipv4, false);
            Console.WriteLine(novoIP.ToString());
            ListaIPs.Add(novoIP);
        }

        
        

        


    }
}
