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
            String entrada = "";
            String[] ipv4 = new String[2];
            Console.WriteLine("Informe o IP e sua máscara. O padrão CIDR e por extenso da máscara são aceitos.");
            Console.WriteLine("Siga os exemplos abaixo:\nCIDR: 192.168.0.54/24\nExtenso: 192.168.0.54 255.255.255.0");

            entrada = Console.ReadLine();
            int tam = entrada.Length;
            if (entrada[tam - 3].Equals('/'))
            {
                ipv4[0] = entrada.Substring(0, tam - 3);

                ipv4[1] = ExtenseParse(int.Parse(entrada.Substring(tam - 2, 2)));
            }
            //ipv4 = entrada.Split();
            foreach (String i in ipv4)
            {
                Console.WriteLine(i);
            }
        }
        public static String ExtenseParse(int CIDR)
        {
            StringBuilder mask = new StringBuilder();

            int qtdCampos = CIDR / 8;
            int resto = CIDR % 8;
            if (resto == 0)
            {
                for (int i = 0; i < qtdCampos; i++)
                {
                    mask.Append("255");
                    if (i == qtdCampos - 1)
                    {
                        break;
                    }
                    mask.Append(".");
                }
                for (int i = 0; i < (4 - qtdCampos); i++)
                {
                    mask.Append(".0");
                }
            }
            else
            {
                for (int i = 0; i < qtdCampos; i++)
                {
                    mask.Append("255");
                    if (i == qtdCampos - 1)
                    {
                        break;
                    }
                    mask.Append(".");
                }
                //uso de função que transforma o resto da divisão em binário     
                int octeto = 8;
                int campo = 0;
                for (int i = octeto; i > 0; i--)
                {
                    campo += (int)Math.Pow(2, octeto);
                }
                mask.Append(campo.ToString());
                qtdCampos++;
                for (int i = 0; i < (4 - qtdCampos); i++)
                {
                    mask.Append(".0");
                }
            }
            return mask.ToString();
        }
    }

}
