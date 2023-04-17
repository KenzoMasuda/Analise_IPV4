using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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

            input = Console.ReadLine().Trim();

            if (ValidaEntrada(input))
            {
                //início da validação do ip 
                //Console.WriteLine(ipv4.ip);
                //Console.WriteLine(ipv4.mask);
                if (!ValidaIP(ipv4.ip))
                {
                    Console.WriteLine(ipv4.ip);
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
                        CalculaIP(ipv4);
                    }
                }
            }
            else if (ipv4.mask == 0) Console.WriteLine("Máscara inválida");
            else Console.WriteLine("A entrada não segue a formatação padrão, digite novamente conforme os exemplos!");

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey(); // espera o usuário pressionar uma tecla
        }

        public static String GetFirstValid(String ipRede)                               //Recebe o IP de Rede
        {                                                                               //Incrementa 1 ao último octeto e retorna como primeiro IP válido
            StringBuilder sb = new StringBuilder();
            sb.Append(ipRede.Substring(0, 12));

            String lastOcteto = ipRede.Substring(ipRede.Length - 3);
            lastOcteto = (int.Parse(lastOcteto) + 1).ToString("D3");
            sb.Append(lastOcteto); //talvez colocar ".' aqui

            return sb.ToString();
        }

        public static String GetLastValid(String ipBroadCast)                           //Recebe o IP de BroadCast
        {                                                                               //Decrementa 1 ao último octeto e retorna como último IP válido
            StringBuilder sb = new StringBuilder();
            sb.Append(ipBroadCast.Substring(0, 12));

            String lastOcteto = ipBroadCast.Substring(ipBroadCast.Length - 3);
            lastOcteto = (int.Parse(lastOcteto) - 1).ToString("D3");
            sb.Append(lastOcteto); //talvez colocar ".' aqui

            return sb.ToString();
        }
        public static void CalculaIpSubRede((String ip, int cidr) ipv4)    
        {
            int qtdOctetosRede = ipv4.cidr / 8;
            int bitsSubRede = ipv4.cidr % 8;
            int qtdDot = qtdOctetosRede - 1;
            if(qtdDot < 0) qtdDot = 0;
            int qtdSR = (int)Math.Pow(2, bitsSubRede);
            long qtdHost = (long)Math.Pow(2, 32 - ipv4.cidr) - 2;
            int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede)); //8 bits, iniciando em 0, portanto o maior expoente é 7
            //int indexOctetoSubRede = qtdOctetosRede * 3 + qtdDot;
            //indexOctetoSubRede = indexOctetoSubRede == 0 ? indexOctetoSubRede : indexOctetoSubRede++;
            int indexOctetoSubRede = (qtdOctetosRede * 3 + qtdDot) == 0 ? 0 : (qtdOctetosRede * 3 + qtdDot)+1;
            String ipRede = GetIpRede(ipv4.ip, qtdSR, indexOctetoSubRede, qtdOctetosRede, bitsSubRede);
            String firstValid = GetFirstValid(ipRede);
            String ipBroadCast = GetIpBroadCast(ipv4.ip, qtdSR, indexOctetoSubRede, qtdOctetosRede, bitsSubRede);
            String lastValid = GetLastValid(ipBroadCast);
            String mask = ToExtenseMask(ipv4.cidr);


            Console.WriteLine("Máscara:      " + mask);
            Console.WriteLine("IP rede:      " + ipRede);
            Console.WriteLine("IP broadcast: " + ipBroadCast);
            Console.WriteLine("First válid:  " + firstValid);
            Console.WriteLine("Last válid:   " + lastValid);
            Console.WriteLine("Hosts:        " + qtdHost);

        }

        public static String GetIpBroadCast(String ip, int qtdSR, int indexOctetoSubRede, int qtdOctetosRede, int bitsSubRede)
        {
            int octetoRede = 0;
            int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede));
            StringBuilder sb = new StringBuilder();

            String octetoBase = ip.Substring(indexOctetoSubRede, 3);         //Octeto a ser calculado para obter o ip de broadcast
            sb.Append(ip.Substring(0, indexOctetoSubRede == 0 ? 0 : indexOctetoSubRede-1));
            for (int i = 0; i < qtdSR; i++)                                  //inicia com o octeto do ip de subrede 0, verificando se a faixa do IP atual está entre um ip de rede ou do próximo
            {
                
                if (int.Parse(octetoBase) >= octetoRede && int.Parse(octetoBase) < (octetoRede + qtdFaixasRede))
                {   //se octetoBase estiver ENTRE dois octetos ou for igual ao primeiro, pega o que veio antes
                    sb.Append(sb.ToString() == "" ? (octetoRede + qtdFaixasRede - 1).ToString("D3") : '.' + (octetoRede + qtdFaixasRede - 1).ToString("D3"));
                    break;
                }
                else if (int.Parse(octetoBase) == (octetoRede + qtdFaixasRede))
                {   //se for igual ao octeto seguinte, atribui o seguinte
                    sb.Append(sb.ToString() == "" ? (octetoRede + qtdFaixasRede * 2 - 1).ToString("D3") : '.' + (octetoRede + qtdFaixasRede * 2 - 1).ToString("D3"));
                    break;
                }
                else octetoRede += qtdFaixasRede;                            //enquanto não chegar, incrementa pulando os octetos de IP de rede
            }
            for (int i = 0; i < 4 - qtdOctetosRede - 1; i++)
            {
                sb.Append(".255");
            }
            return sb.ToString();
        }


        public static String GetIpRede(String ip, int qtdSR, int indexOctetoSubRede, int qtdOctetosRede, int bitsSubRede)
        {
            int octetoRede = 0;
            int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede));
            StringBuilder sb = new StringBuilder();
            String octetoBase = ipv4.ip.Substring(indexOctetoSubRede, 3); //Octeto a ser calculado para obter o ip de rede
            sb.Append(ip.Substring(0, indexOctetoSubRede == 0 ? 0 : indexOctetoSubRede-1));
            for (int i = 0; i < qtdSR; i++)                                  //inicia com o ip de rede 0, verificando se a faixa do IP atual está entre um ip de rede ou do próximo
            {
                if (int.Parse(octetoBase) >= octetoRede && int.Parse(octetoBase) < (octetoRede + qtdFaixasRede))
                {   //se octetoBase estiver ENTRE dois octetos ou for igual ao primeiro, pega o que veio antes
                    sb.Append(sb.ToString() == "" ? octetoRede.ToString("D3") : ("." + octetoRede.ToString("D3")));
                    break;
                }
                else if(int.Parse(octetoBase) == (octetoRede + qtdFaixasRede))
                {   //se for igual ao octeto seguinte, atribui o seguinte
                    sb.Append(sb.ToString() == "" ? (octetoRede + qtdFaixasRede).ToString("D3") : ('.' + (octetoRede + qtdFaixasRede).ToString("D3")));
                    break;
                }
                else octetoRede += qtdFaixasRede;                            //enquanto não chegar, incrementa pulando os octetos de IP de rede
            }
            
            for (int i = 0; i < 4 - qtdOctetosRede - 1; i++)          
            {
                sb.Append(".000");
            }
            return sb.ToString();
        }

        public static void CalculaIpPadrao((String ip, int cidr) ipv4)
        {
            int qtdOctetosRede = ipv4.cidr / 8;
            int qtdDot = qtdOctetosRede - 1; //quantidade de pontos entre os octetos
            int qtdZeros = 32 - ipv4.cidr;
            String mask;
            String ipRede;
            String firstValid;
            String ipBroadCast;
            String lastValid;
            int qtdSR = 0;
            long qtdHost = (long)(Math.Pow(2, qtdZeros)) - 2;

            StringBuilder sb = new StringBuilder();

            mask = ToExtenseMask(ipv4.cidr);

            sb.Append(ipv4.ip.Substring(0, qtdOctetosRede *3 + qtdDot)); //Inclui octetos fixos no SB
            for(int i = 0; i< 4 - qtdOctetosRede; i++)                   //Preenche octetos restantes com "000";
            {
                sb.Append(".000");
            }
            ipRede = sb.ToString();

            sb.Remove(sb.Length - 1, 1);
            sb.Insert(sb.Length, '1');                               //Troca último caractere do ip de rede por '1', como se estivesse somando 1
            firstValid = sb.ToString();
            sb.Clear();

            sb.Append(ipv4.ip.Substring(0, qtdOctetosRede *3 + qtdDot)); //Inclui octetos fixos no SB
            for(int i = 0; i < 4 - qtdOctetosRede; i++)                  //Preenche octetos restantes com "255";
            {                                                            //PS: processo pode ser alterado, para n recomeçar a construção toda vez
                sb.Append(".255");                                       //Ps: excluir parte do ip que não pé fixo e preencher com 255 somente
            }
            ipBroadCast = sb.ToString();

            sb.Remove(sb.Length - 1, 1);
            sb.Insert(sb.Length, '4');                              //Troca último caractere do ip de rede por '4', como se estivesse subtraindo 1
            lastValid = sb.ToString();
            sb.Clear();

            Console.WriteLine("Máscara:      " + mask);
            Console.WriteLine("IP rede:      " + ipRede);
            Console.WriteLine("IP broadcast: " + ipBroadCast);
            Console.WriteLine("First válid:  " + firstValid);
            Console.WriteLine("Last válid:   " + lastValid);
            Console.WriteLine("Hosts:        " + qtdHost);
        }

        public static void CalculaIP((String ip, int mask) ipv4)             //Recebe a entrada (ip+mask)
        {                                                                    //Se for IP de Classe padrão, chama CalculaIpPadrao();
            if (ipv4.mask == 8 || ipv4.mask == 16 || ipv4.mask == 24)        //Senao, chama CalculaIpSubRede();
            {
                CalculaIpPadrao(ipv4);
            }
            else CalculaIpSubRede(ipv4);
        }

        public static String ToExtenseMask(int cidr)           //Recebe o CIDR do IP
        {                                                      //Calcula qtd de Octetos fixos (Rede), como também a qtd de Bits do octeto de subrede, caso houver 
            StringBuilder mask = new StringBuilder();          //Completa os octetos fixos com "255"
            int qtdOctetosRede = cidr / 8;                     //Se houver octeto de subrede, transformar os bits de subrede para decimal com getOctetForDecimal
            int qtdBitRede = cidr % 8;                         //Se houver, preenche octetos restantes com "000"
                                                               //Retorna máscara por extenso
            for (int i = 0; i < qtdOctetosRede; i++)
            {
                mask.Append("255");
                if (i == qtdOctetosRede - 1) break;
                mask.Append(".");
            }
            if (qtdBitRede != 0)
            { 
                mask.Append(mask.ToString() == "" ? GetOctetForDecimal(qtdBitRede) : "." + GetOctetForDecimal(qtdBitRede));
                qtdOctetosRede++;
            }
            for (int i = 0; i < (4 - qtdOctetosRede); i++)
            {
                mask.Append(".000");
            }
            return mask.ToString();

        }
        public static String GetOctetForDecimal(int bitsRede)        //Recebe a quantidade de bits setados com '1' de um octeto, 
        {                                                            //Passa para decimal
            int octeto = 7;
            int campo = 0;
            for (int i = octeto; i > (octeto - bitsRede); i--)
                campo += (int)Math.Pow(2, i);
            return campo.ToString();
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
                if (int.TryParse(octString, out octInt)) {
                    if (octInt < 0 || octInt > 255) valid = false;
                }
                else return valid = false;
            }
            return valid;
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
                foreach(String octeto in octetos)       //Converte octetos de decimal para binário
                {
                    octetosBit[index] = Convert.ToString(int.Parse(octeto), 2); 
                    index++;
                }
                foreach(String octetoBit in octetosBit) maskBit += octetoBit; //concatena os resultados dos octetos binários  

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

        public static bool ValidaEntrada(String input)                //Retorna true se o formato padrão de entrada estiver correto 
        {                                                             //Analisa se entrada está em CIDR ou por extenso   
            int tam = input.Length;                                   //Valida a máscara em ToCidrMask() e caso entrar já em CIDR
            int cidr = 0;
            if (input[tam - 3].Equals('/'))                           //Caso o CIDR for menor/igual que 0 ou igual/maior que 31, cidr = 0; 
            {
                ipv4.ip = input.Substring(0, tam - 3);
                //ipv4.mask = int.TryParse(input.Substring(tam - 2, 2), out _);
                bool sucess = int.TryParse(input.Substring(tam - 2, 2), out cidr);
                if (cidr >= 0 && cidr < 31)
                    ipv4.mask = cidr;
                else ipv4.mask = 0;
            }                                                        
            else if (input[tam - 16].Equals(' '))
            {
                ipv4.ip = input.Substring(0, tam - 16);
                ipv4.mask = ToCidrMask(input.Substring(tam - 15));
                //if (ipv4.mask == 0) return false;
            }
            else return false;

            return true;
        }

        
        
    }

}
