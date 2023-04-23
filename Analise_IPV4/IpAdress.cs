using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Analise_IPV4
{
    internal class IpAddress
    {
        public String ip { get; set; }
        public String mask { get; set; }
        public int maskCidr { get; set; }
        public String ipRede { get; set; }
        public String ipBroadCast { get; set; }
        public String firstValid { get; set; }
        public String lastValid { get; set; }
        public int qtdSR { get; set; }
        public long qtdHost { get; set; }

        internal void IpAddressPadrao(String ip, int cidr)
        {
            int qtdOctetosRede = cidr / 8;
            int qtdDot = qtdOctetosRede - 1; //quantidade de pontos entre os octetos
            int qtdZeros = 32 - cidr;

            this.qtdSR = 0;
            this.qtdHost = (long)(Math.Pow(2, qtdZeros)) - 2;

            StringBuilder sb = new StringBuilder();

            this.mask = ToExtenseMask(cidr);

            sb.Append(ip.Substring(0, qtdOctetosRede * 3 + qtdDot)); //Inclui octetos fixos no SB
            for (int i = 0; i < 4 - qtdOctetosRede; i++)             //Preenche octetos restantes com "000";
            {
                sb.Append(".000");
            }
            this.ipRede = sb.ToString();

            sb.Remove(sb.Length - 1, 1);
            sb.Insert(sb.Length, '1');                               //Troca último caractere do ip de rede por '1', como se estivesse somando 1
            this.firstValid = sb.ToString();
            sb.Clear();

            sb.Append(ip.Substring(0, qtdOctetosRede * 3 + qtdDot)); //Inclui octetos fixos no SB
            for (int i = 0; i < 4 - qtdOctetosRede; i++)             //Preenche octetos restantes com "255";
            {                                                        //PS: processo pode ser alterado, para n recomeçar a construção toda vez
                sb.Append(".255");                                   //Ps: excluir parte do ip que não pé fixo e preencher com 255 somente
            }
            this.ipBroadCast = sb.ToString();

            sb.Remove(sb.Length - 1, 1);
            sb.Insert(sb.Length, '4');                              //Troca último caractere do ip de rede por '4', como se estivesse subtraindo 1
            this.lastValid = sb.ToString();
            sb.Clear();
        }
        public IpAddress((String ipEntrada, int maskEntrada) ipv4Entrada, Boolean isSubRede)
        {
            
            this.ip = ipv4Entrada.ipEntrada;
            this.maskCidr = ipv4Entrada.maskEntrada;

            if (isSubRede)
            {
                int qtdOctetosRede = maskCidr / 8;
                int bitsSubRede = maskCidr % 8;
                int qtdDot = qtdOctetosRede - 1;
                if (qtdDot < 0) qtdDot = 0;
                int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede)); //8 bits, iniciando em 0, portanto o maior expoente é 7
                int indexOctetoSubRede = (qtdOctetosRede * 3 + qtdDot) == 0 ? 0 : (qtdOctetosRede * 3 + qtdDot) + 1;

                this.qtdSR = (int)Math.Pow(2, bitsSubRede);
                this.qtdHost = (long)Math.Pow(2, 32 - maskCidr) - 2;
                this.mask = ToExtenseMask(maskCidr);
                this.ipRede = GetIpRede(ip, qtdSR, indexOctetoSubRede, qtdOctetosRede, bitsSubRede);
                this.ipBroadCast = GetIpBroadCast(ip, qtdSR, indexOctetoSubRede, qtdOctetosRede, bitsSubRede);
                this.firstValid = GetFirstValid(this.ipRede);
                this.lastValid = GetLastValid(this.ipBroadCast);
            }
            else IpAddressPadrao(ip, maskCidr);
        }

        protected String GetIpRede(String ip, int qtdSR, int indexOctetoSubRede, int qtdOctetosRede, int bitsSubRede)
        {
            int octetoRede = 0;
            int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede));
            StringBuilder sb = new StringBuilder();
            String octetoBase = ip.Substring(indexOctetoSubRede, 3); //Octeto a ser calculado para obter o ip de rede
            sb.Append(ip.Substring(0, indexOctetoSubRede == 0 ? 0 : indexOctetoSubRede - 1));
            for (int i = 0; i < qtdSR; i++)                                  //inicia com o ip de rede 0, verificando se a faixa do IP atual está entre um ip de rede ou do próximo
            {
                if (int.Parse(octetoBase) >= octetoRede && int.Parse(octetoBase) < (octetoRede + qtdFaixasRede))
                {   //se octetoBase estiver ENTRE dois octetos ou for igual ao primeiro, pega o que veio antes
                    sb.Append(sb.ToString() == "" ? octetoRede.ToString("D3") : ("." + octetoRede.ToString("D3")));
                    break;
                }
                else if (int.Parse(octetoBase) == (octetoRede + qtdFaixasRede))
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

        protected String GetFirstValid(String ipRede)                               //Recebe o IP de Rede
        {                                                                               //Incrementa 1 ao último octeto e retorna como primeiro IP válido
            StringBuilder sb = new StringBuilder();
            sb.Append(ipRede.Substring(0, 12));

            String lastOcteto = ipRede.Substring(ipRede.Length - 3);
            lastOcteto = (int.Parse(lastOcteto) + 1).ToString("D3");
            sb.Append(lastOcteto);

            return sb.ToString();
        }

        protected String GetIpBroadCast(String ip, int qtdSR, int indexOctetoSubRede, int qtdOctetosRede, int bitsSubRede)
        {
            int octetoRede = 0;
            int qtdFaixasRede = (int)Math.Pow(2, (8 - bitsSubRede));
            StringBuilder sb = new StringBuilder();

            String octetoBase = ip.Substring(indexOctetoSubRede, 3);         //Octeto a ser calculado para obter o ip de broadcast
            sb.Append(ip.Substring(0, indexOctetoSubRede == 0 ? 0 : indexOctetoSubRede - 1));
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

        protected String GetLastValid(String ipBroadCast)                           //Recebe o IP de BroadCast
        {                                                                               //Decrementa 1 ao último octeto e retorna como último IP válido
            StringBuilder sb = new StringBuilder();
            sb.Append(ipBroadCast.Substring(0, 12));

            String lastOcteto = ipBroadCast.Substring(ipBroadCast.Length - 3);
            lastOcteto = (int.Parse(lastOcteto) - 1).ToString("D3");
            sb.Append(lastOcteto);

            return sb.ToString();
        }

        protected static String ToExtenseMask(int cidr)           //Recebe o CIDR do IP
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("--------------Relatório-------------");
            sb.AppendLine("IP calculado:       " + ip);
            sb.AppendLine("Máscara:            " + mask);
            sb.AppendLine("IP Rede:            " + ipRede);
            sb.AppendLine("IP BroadCast:       " + ipBroadCast);
            sb.AppendLine("Primeiro IP válido: " + firstValid);
            sb.AppendLine("Último IP válido:   " + lastValid);
            sb.AppendLine("Qtd SubRedes:       " + qtdSR);
            sb.AppendLine("Qtd hosts:          " + qtdHost);

            return sb.ToString();
        }

    }
}
