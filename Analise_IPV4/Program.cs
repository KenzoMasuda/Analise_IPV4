using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Analise_IPV4
{
    internal class Program
    {
        static (String ip, String mask) ipv4 = ("", "");
        static void Main(string[] args)
        {
            String input = "";
            //String[] ipv4 = new String[2]; // utilizar tupla?
            //(String ip, String mask) ipv4 = ("","");
            Console.WriteLine("Informe o IP e máscara. O padrão CIDR e por extenso da máscara são aceitos.");
            Console.WriteLine("Siga os exemplos abaixo:\nCIDR: 192.168.000.054/24\nExtenso: 192.168.000.054 255.255.255.000");

            input = Console.ReadLine();
            int tam = input.Length;
            Console.WriteLine(tam);
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
                    if (ipv4.ip.Equals("000.000.000.000") || ipv4.ip.Equals("255.255.255.255"))
                        Console.WriteLine("Endereço IP inválido!");
                    if (ipv4.mask.Equals("255.255.255.255"))
                        Console.WriteLine("Máscara inválida!");
                    //função para calculos do ipv4, tipo void, printa resultados na tela
                    //criar looping para que o usuário possa solicitar o calculo de quantos ips desejar
                    //finalizar programa com entrada == "sair";
                }
                //função para validar ip e máscara - argumento, ipv4
                //  após validar ipv4, iniciar cálculo do ipv4 para descobrir ip de rede, 1° e último válido, broadcast, identificar
                //  qual subrede se trata das n subredes possibilitadas pela máscara, qtd de hosts dessa subrede;
            }
            else Console.WriteLine("Entrada inválida, digite novamente conforme os exemplos!");



            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey(); // espera o usuário pressionar uma tecla
        }

        public static bool ValidaIP((String ip, String mask) ipv4)
        {
            String[] octetosIP = ipv4.ip.Split('.');
            String[] octetosMask = ipv4.mask.Split('.');
            bool valid = true;
            int octInt;

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

            return valid;
;        }

        public static bool ValidaEntrada(String input)
        {
            int tam = input.Length;
            if (input[tam - 3].Equals('/')) 
            {
                ipv4.ip = input.Substring(0, tam - 3);
                ipv4.mask = ToExtenseMask(int.Parse(input.Substring(tam - 2, 2)));
            }
            else if (input[tam - 16].Equals(' '))
            {
                ipv4.ip = input.Substring(0, tam - 16);
                ipv4.mask = input.Substring(tam - 15);
            }
            else return false;

            return true;
        }
        public static String ToExtenseMask(int cidr) //status de conclusão = 100% - em funcionamento
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
