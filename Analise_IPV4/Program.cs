using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
//using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Analise_IPV4
{
    internal class Program
    {
        static (String ip, int mask) ipv4 = ("", 0);
        static void Main(string[] args)
        {
            String input = "";
     
            Console.WriteLine("Informe o IP e máscara. O padrão CIDR e por extenso da máscara são aceitos.");
            Console.WriteLine("Siga os exemplos abaixo:\nCIDR: 192.168.000.054/08\nExtenso: 192.168.000.054 255.000.000.000");

            input = Console.ReadLine();

            if (ValidaEntrada(input))
            {
                //início da validação do ip 
                Console.WriteLine(ipv4.ip);
                Console.WriteLine(ipv4.mask);
                if (!ValidaIP(ipv4.ip))
                {
                    Console.WriteLine("Endereço IP apresenta erro lógico!");
                }
                else
                {
                    if (ipv4.ip.Equals("000.000.000.000") || ipv4.ip.Equals("255.255.255.255"))
                    {
                        Console.WriteLine("Endereço IP inválido!");
                    }
                    else
                    {
                        Console.WriteLine("Entrou aqui");
                        CalculaIP(ipv4);
                    }
                }

            }
            else if (ipv4.mask == 0) Console.WriteLine("Máscara inválida");
            else Console.WriteLine("A entrada não segue a formatação padrão, digite novamente conforme os exemplos!");

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey(); // espera o usuário pressionar uma tecla
        }

        public static void CalculaIpSubRede((String ip, int cidr) ipv4)
        {
            int octetoRede = 0;
            int qtdOctetosRede = ipv4.cidr / 8;
            int bitsSubRede = ipv4.cidr % 8;
            int qtdDot = qtdOctetosRede - 1;
            int qtdSR = (int)Math.Pow(2, bitsSubRede);
            long qtdHosts = (long)Math.Pow(2, 32 - ipv4.cidr);
            int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede)); //8 bits, iniciando em 0, portanto o maior expoente é 7

            int indexOctetoSubRede = qtdOctetosRede * 3 + qtdDot;

            String octetoBase = ipv4.ip.Substring(indexOctetoSubRede+1, 3);
            StringBuilder sb = new StringBuilder(); // criar função que retorna ip de rede

            String ipRede = GetIpRede(ipv4.ip, qtdSR, indexOctetoSubRede, qtdOctetosRede, bitsSubRede);

            sb.Append(ipRede);
            sb[sb.Length - 1] = '1';
            String firstValid = sb.ToString();

            sb.Clear();

        }

        public static String GetIpRede(String ip, int qtdSR, int indexOctetoSubRede, int qtdOctetosRede, int bitsSubRede)
        {
            int octetoRede = 0;
            int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede));
            StringBuilder sb = new StringBuilder();

            String octetoBase = ipv4.ip.Substring(indexOctetoSubRede + 1, 3);
            sb.Append(ip.Substring(0, indexOctetoSubRede));
            Console.WriteLine("Octeto a ser calculado para obter o ip de rede: " + octetoBase);
            for (int i = 0; i < qtdSR; i++)
            {
                if (int.Parse(octetoBase) > octetoRede && int.Parse(octetoBase) < (octetoRede + qtdFaixasRede))
                {
                    sb.Append("." + octetoRede.ToString("D3"));
                    break;
                }
                else octetoRede += qtdFaixasRede;
            }
            //Console.WriteLine(sb.ToString());
            for (int i = 0; i < 4 - qtdOctetosRede - 1; i++)
            {
                sb.Append(".000");
            }
            return sb.ToString();
        }

        public static void CalculaIpPadrao((String ip, int cidr) ipv4)
        {
            int octetosRede = ipv4.cidr / 8;
            int qtdDot = octetosRede - 1; //quantidade de pontos entre os octetos
            int qtdZeros = 32 - ipv4.cidr;
            StringBuilder sb = new StringBuilder();

            sb.Append(ipv4.ip.Substring(0, octetosRede*3 + qtdDot)); //obtenção do ip de rede
            for(int i = 0; i< 4 - octetosRede; i++)
            {
                sb.Append(".000");
            }
            String ipRede = sb.ToString();

            sb[sb.Length - 1] = '1'; //obtenção 1° ip válido
            String firstValid = sb.ToString();
            sb.Clear();

            sb.Append(ipv4.ip.Substring(0, octetosRede*3 + qtdDot)); //obtenção do ip de broadcast
            for(int i = 0; i < 4 - octetosRede; i++)
            {
                sb.Append(".255");
            }
            String ipBroadCast = sb.ToString();

            sb[sb.Length - 1] = '4'; //obtenção last ip válido
            String lastValid = sb.ToString();
            sb.Clear();

            String mask = ToExtenseMask(ipv4.cidr);
            long qtdHost = (long)(Math.Pow(2, qtdZeros)) - 2;
            int qtdSR = 0;
            Console.WriteLine("Máscara: " + mask);
            Console.WriteLine("IP rede: " + ipRede);
            Console.WriteLine("IP broadcast: " + ipBroadCast);
            Console.WriteLine("First válid: " + firstValid);
            Console.WriteLine("Last válid: " + lastValid);
            Console.WriteLine("Quantidade de Hosts: " + qtdHost);
            //Console.WriteLine("Quantidade de SubRedes: " + qtdSR);

        }

        public static void CalculaIP((String ip, int mask) ipv4)
        {
            if (ipv4.mask == 8 || ipv4.mask == 16 || ipv4.mask == 24)
            {
                CalculaIpPadrao(ipv4);
            }
            else CalculaIpSubRede(ipv4);
        }

        public static String ToExtenseMask(int cidr) //atualmente usada no começo do programa, será agora utilizada no final para imprimir resultado
        {                                            //Função que pega o valor do CIDR e retorna uma String
            StringBuilder mask = new StringBuilder();//que representa a máscara em formato por extenso

            int qtdOctetosCompletos = cidr / 8;
            int qtdBitRede = cidr % 8;

            for (int i = 0; i < qtdOctetosCompletos; i++)
            {
                mask.Append("255");
                if (i == qtdOctetosCompletos - 1) break;
                mask.Append(".");
            }
            if (qtdBitRede != 0)
            {
                mask.Append("." + getOctetForDecimal(qtdBitRede));
                qtdOctetosCompletos++;
            }
            for (int i = 0; i < (4 - qtdOctetosCompletos); i++)
            {
                mask.Append(".000");
            }
            return mask.ToString();

        }
        public static String getOctetForDecimal(int bitsRede)
        { //minha implementação
            int octeto = 7;
            int campo = 0;
            for (int i = octeto; i > (octeto - bitsRede); i--)
            {
                campo += (int)Math.Pow(2, i);
                //Console.WriteLine(campo);
            }
            return campo.ToString();
        }

        public static bool ValidaIP(String ip)
        {
            bool valid = false;
            String[] octetosIP = ip.Split('.');
           
            Console.WriteLine("Analisando endereço IP...");
            if (ValidaOctetos(octetosIP))
            {
                valid = true;
                Console.WriteLine("Endereço IP segue o padrão!");
            }
            return valid;
        }

        public static bool ValidaOctetos(String[] Octetos)
        {
            bool valid = true;
            int octInt;
            foreach (String octString in Octetos) //Percorre IP, se houver erro finaliza função retornando false
            {
                if (int.TryParse(octString, out octInt))
                {
                    Console.WriteLine("Valor do octeto: " + octInt);
                    if (octInt < 0 || octInt > 255) return valid = false;
                }
                else return valid = false;
            }
            return valid;
        }

        public static String ValidaMask(String mask)
        {
            String[] octetos = mask.Split('.');
            String maskBit = "";
            
            if (ValidaOctetos(octetos))
            {
                String[] octetosBit = new string[4];
                String maskBitRede = "";
                String maskBitHost = "";
                int index = 0;
                foreach(String octeto in octetos)
                {
                    //Console.WriteLine("Valor do octeto string: " + octeto);
                    octetosBit[index] = Convert.ToString(int.Parse(octeto), 2); // Converte um inteiro para binário em formato String
                    //Console.WriteLine("Valor do octeto binario: " + octetosBit[index]);
                    index++;
                }
                foreach(String octetoBit in octetosBit) //concatena os resultados dos octetos binários
                {
                    //Console.WriteLine("valor de maskBit: " + maskBit);
                    maskBit += octetoBit;
                    //Console.WriteLine("valor de maskBit pós concatenação: " + maskBit);
                }
                //Console.WriteLine("MaskBit final: " + maskBit);
                int indexFinalRede = maskBit.IndexOf("0");
                maskBitRede = maskBit.Substring(0, indexFinalRede);
                maskBitHost = maskBit.Substring(indexFinalRede);
                //Console.WriteLine("Parte rede: " + maskBitRede);
                //Console.WriteLine("Parte host: " + maskBitHost);
                return (maskBitRede.Contains("0") || maskBitHost.Contains("1")) ? "0" : maskBit; //código abaixo mantido para melhor compreensão
                /*if (maskBitRede.Contains("0") || maskBitHost.Contains("1")) Console.WriteLine("Máscara inválida!!!");
                else return maskBit; // utilizar operador ternário posteriormente
                */
            }
           return maskBit = "0";
        }

        public static int ToCidrMask(String mask)
        {
            int cidr = 0;
            String maskBit = ValidaMask(mask);
            if (!maskBit.Equals("0"))
            {
                if (maskBit.Contains("0"))
                {
                    cidr = maskBit.IndexOf('0');
                }
                //else cidr = 32;
            }
            if (cidr == 31) cidr = 0;
            return cidr;
        }

        public static bool ValidaEntrada(String input) //retorna true se o formato padrão de entrada estiver correto e já valida a máscara
        {
            int tam = input.Length;
            if (input[tam - 3].Equals('/')) 
            {
                ipv4.ip = input.Substring(0, tam - 3);
                //ipv4.mask = ToExtenseMask(int.Parse(input.Substring(tam - 2, 2))); //retirar função e manter formato CIDR
                ipv4.mask = int.Parse(input.Substring(tam - 2, 2));
            }                                                        
            else if (input[tam - 16].Equals(' '))
            {
                ipv4.ip = input.Substring(0, tam - 16);
                //ipv4.mask = input.Substring(tam - 15); //a ser criado ToCidrMask;
                ipv4.mask = ToCidrMask(input.Substring(tam - 15));
                if (ipv4.mask == 0) return false;
            }
            else return false;

            return true;
        }

        
        
    }

}
