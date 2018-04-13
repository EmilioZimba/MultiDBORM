### Projecto inacabado

# MultiDBORM
Mini ORM que suporta qualquer BD relacional para C#


# Exemplo de Utilização

Seja um objecto contacto que manipula os dados da tabela contactos

classe


    [Tabela("contactos")]
    public class Contacto
    {
        [Coluna("cod")]
        public int cod { get; set; }

        [Coluna("contacto")]
        public string contacto { get; set; }

        [Coluna("tipo")]
        public string tipo { get; set; }

        /**
         * Essa propriedade deve ser do tipo string pk faco o uso de palavras reservadas do RDBMS(MySql) nela
         * */
        [Coluna("proprietario", Tipo=TipoDado.Inteiro, FK=true)]
        public string proprietario { get; set; }
    }



## Como gravar


        public bool Cadastrar(Contacto c)
        {
            bool resp = false;
            using (Gerar g = new Gerar())
            {
                resp = new Conexao().Executar(new MySqlCRUD().inserir(g.Tabela(typeof(Contacto)), g.CamposSemPK(r), g.ValoresSemPK(r)));
            }
            return resp;
        }
