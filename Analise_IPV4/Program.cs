using System;
using System.Collections.Generic;
using System.Linq;
//using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Analise_IPV4
{
    internal class Program
    {
        //static (String ip, String mask) ipv4 = ("", "");
        static (String ip, int mask) ipv4 = ("", 0);
        static void Main(string[] args)
        {
            String input = "";
            //String[] ipv4 = new String[2]; // utilizar tupla?
            //(String ip, String mask) ipv4 = ("","");
            Console.WriteLine("Informe o IP e máscara. O padrão CIDR e por extenso da máscara são aceitos.");
            Console.WriteLine("Siga os exemplos abaixo:\nCIDR: 192.168.000.054/24\nExtenso: 192.168.000.054 255.255.255.000");

            input = Console.ReadLine();
        
            if (ValidaEntrada(input))
            {
                //início da validação do ip 
                Console.WriteLine(ipv4.ip);
                Console.WriteLine(ipv4.mask);
                if (!ValidaIP(ipv4))
                {
                    Console.WriteLine("Endereço IP e/ou máscara apresenta erro lógico!");
                }
                else
                {
                    bool calcular = true;
                    if (ipv4.ip.Equals("000.000.000.000") || ipv4.ip.Equals("255.255.255.255"))
                    {
                        calcular = false;
                        Console.WriteLine("Endereço IP inválido!");
                    }
                    if (ipv4.mask.Equals("255.255.255.255") || ipv4.mask.Equals("255.255.255.254"))
                    {
                        calcular = false;
                        Console.WriteLine("Máscara inválida!");
                    }

                    if (calcular)
                    {
                        //CalculaIP(ipv4);
                    }
                        

                    //função para calculos do ipv4, tipo void, printa resultados na tela
                    //criar looping para que o usuário possa solicitar o calculo de quantos ips desejar
                    //finalizar programa com entrada == "sair";
                }
                
            }
            else Console.WriteLine("Entrada inválida, digite novamente conforme os exemplos!");



            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey(); // espera o usuário pressionar uma tecla
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
            String valid = "0";
            String[] octetos = mask.Split('.');
            String[] octetosBit = new string[4];
            String maskBit = "";
            String maskBitRede = ""; //talvez tirar daqui e colocar dentro do if
            String maskBitHost = "";
            if (ValidaOctetos(octetos))
            {
                int index = 0;
                foreach(String octeto in octetos)
                {
                    octetosBit[index] = Convert.ToString(int.Parse(octeto), 2); // Converte um inteiro para binário em formato String
                    index++;
                }
                foreach(String octetoBit in octetosBit)
                {
                    maskBit += octetoBit;
                }
                Console.WriteLine(maskBit);
                int indexFinalHost = maskBit.IndexOf("0");
                maskBitRede = maskBit.Substring(0, indexFinalHost);
                maskBitHost = maskBit.Substring(indexFinalHost);
                Console.WriteLine("Parte rede: " + maskBitRede);
                Console.WriteLine("Parte host: " + maskBitHost);
                if (maskBitRede.Contains("0") || maskBitHost.Contains("1")) Console.WriteLine("Máscara inválida!!!");
                else valid = maskBit; // utilizar operador ternário posteriormente
            }
            return valid;
        }

        public static int ToCidrMask(String mask)
        {
            int cidr = 0;

            if (!ValidaMask(mask).Equals("0"))
            {
                Console.WriteLine("Função em desenvolvimento!!!");
            }
            return cidr;
        }

        public static void CalculaIP((String ip, String mask) ipv4)
        {
            /*
                Classe
                ip de rede 
                ip de broadcast
                1° válido
                Último válido
                qtd hosts
                qtd subredes
            */
        }
        public static bool ValidaIP((String ip, int mask) ipv4)
        {
            bool valid = true;
            int octInt;
            /*String[] octetosIP = ipv4.ip.Split('.');
            String[] octetosMask = ipv4.mask.Split('.');
            

            //bloco de código abaixo será substituido por
            //validaOctetos(ipv4.ip)
            //validaOctetos(ipv4.mask);
            Console.WriteLine("Analisando endereço IP...");
            foreach (String octString in octetosIP) //Percorre IP, se houver erro finaliza função retornando false
            {
                if (int.TryParse(octString, out octInt))
                {
                    Console.WriteLine(octInt);
                    if (octInt < 0 || octInt > 255) return valid = false;
                }
                else return valid = false;
            }
            Console.WriteLine("Endereço IP segue o padrão!");
            Console.WriteLine("Analisando máscara...");
            foreach (String octString in octetosMask) //Percorre máscara, se houver erro finaliza função retornando false
            {
                if (int.TryParse(octString, out octInt))
                {
                    Console.WriteLine(octInt);
                    if (octInt < 0 || octInt > 255) return valid = false;
                }
                else return valid = false;

            }
            Console.WriteLine("Máscara segue o padrão!");
            */
            return valid;
;        }



        public static bool ValidaEntrada(String input)
        {
            int tam = input.Length;
            if (input[tam - 3].Equals('/')) 
            {
                ipv4.ip = input.Substring(0, tam - 3);
                //ipv4.mask = ToExtenseMask(int.Parse(input.Substring(tam - 2, 2))); //retirar função e manter formato CIDR
                ipv4.mask = int.Parse(input.Substring(tam - 2, 2));
            }                                                                      //primeiro criar função que transforma extense em cidr
            else if (input[tam - 16].Equals(' '))
            {
                ipv4.ip = input.Substring(0, tam - 16);
                //ipv4.mask = input.Substring(tam - 15); //a ser criado ToCidrMask;
                ipv4.mask = ToCidrMask(input.Substring(tam - 15));
            }
            else return false;

            return true;
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
            if(qtdBitRede != 0)
            {
                mask.Append("." + getOctetInDecimal(qtdBitRede));
                qtdOctetosCompletos++;
            }
            for (int i = 0; i < (4 - qtdOctetosCompletos); i++)
            {
                mask.Append(".000");
            }
            return mask.ToString();
            
        }
        public static String getOctetInDecimal(int bitsRede){ //minha implementação
            int octeto = 7;
            int campo = 0;
            for (int i = octeto; i > (octeto-bitsRede); i--)
            {
                campo += (int)Math.Pow(2, i);
                //Console.WriteLine(campo);
            }
            return campo.ToString();
        }
        
    }

}
