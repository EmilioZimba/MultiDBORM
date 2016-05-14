/**
 * Esta classe define as propriedades de um modelo.
 * Ela especifica o nome da tabela e o seu nome alternativo(Alias, vulgarmente conhecido como sufixo do AS no SQL)
 * */

using System;

namespace Mapeamento
{
    //so pode ser aplicada a classes porque cada tabela (ou entidade) representa uma classe (ou modelo)
    [System.AttributeUsage(System.AttributeTargets.Class)] 
    public class TabelaAttribute : Attribute
    {
        string tabela;
        string alias;
        bool view;
        public TabelaAttribute(string tabela)
        {
            this.tabela = tabela;
            alias = tabela;
            view = false;
        }

        public string Tabela
        {
            get { return tabela; }
            set { tabela = value; }
        }

        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        public bool View
        {
            get { return view; }
            set { view = value; }
        }
    }
}