using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analise_IPV4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            String input = "";
            String[] ipv4 = new String[2]; // utilizar tupla?
            Console.WriteLine("Informe o IP e sua máscara. O padrão cidr e por extenso da máscara são aceitos.");
            Console.WriteLine("Siga os exemplos abaixo:\ncidr: 192.168.0.54/24\nExtenso: 192.168.0.54 255.255.255.0");

            input = Console.ReadLine();
            int tam = input.Length;
            if (input[tam - 3].Equals('/')) //Criar função para realizar esta validãção do tipo de padrão utilizado
            {
                ipv4[0] = input.Substring(0, tam - 3);
                ipv4[1] = ToExtenseMask(int.Parse(input.Substring(tam - 2, 2)));
            }
            //ipv4 = input.Split();
            foreach (String i in ipv4)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey(); // espera o usuário pressionar uma tecla
        }
        public static String ToExtenseMask(int cidr) //status de conclusão = 100% - em funcionamento
        {
            StringBuilder mask = new StringBuilder();

            int qtdOctetosCompletos = cidr / 8;
            int qtdBitRede = cidr % 8;

            for (int i = 0; i < qtdOctetosCompletos; i++)
            {
                mask.Append("255");
                if (i == qtdOctetosCompletos - 1)
                {
                    break;
                }
                mask.Append(".");
            }
            if(qtdBitRede != 0)
            {
                mask.Append("." + getOctetInDecimal(qtdBitRede));
                qtdOctetosCompletos++;
            }
            for (int i = 0; i < (4 - qtdOctetosCompletos); i++)
            {
                mask.Append(".0");
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
