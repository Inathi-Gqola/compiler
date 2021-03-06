using static compiler.CompilerCodes;

namespace compiler
{
    public class Syntactic
    {
        CompilerCodes token;
        string lexema;
        string linhaFonte;
        int linhaAtual;
        int colunaAtual;
        private Compiler compiler;

        public Syntactic(Compiler compiler)
        {
            this.compiler = compiler;
        }

        public void analiseSintatica()
        {
            p();
        }

        // <P> ::= '}}' <comandos> '{{'  
        private void p()
        {
            compiler.semantic.regraSemantica(0);

            if (token == T_ABREPROG)
            {
                compiler.lexicon.buscaProximoToken();
                comandos();
                if (token == T_FECHAPROG)
                {
                    compiler.lexicon.buscaProximoToken();
                    compiler.semantic.regraSemantica(1);
                }
                else
                {
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nFECHA PROG {{ esperado, mas encontrei: " + lexema);
                }
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nABRE PROG }} esperado, mas encontrei: " + lexema);
            }
        }

        // <comandos> ::= <comando> 'CABO' <comandos>
        //            |   <comando>    
        private void comandos()
        {
            comando();
            if (token == T_CABO)
            {
                compiler.lexicon.buscaProximoToken();
                comandos();
            }
        }
        // Exemplo sem recursividade
        private void comandosAlternativo()
        {
            comando();
            while (token != T_CABO)
            {
                compiler.lexicon.buscaProximoToken();
                comando();
            }
        }

        // <comando> ::= <cmd_atribuicao>
        //           |   <cmd_escrita> 
        //           |   <cmd_leitura>
        private void comando()
        {
            switch (token)
            {
                case T_ID: comandoAtribuicao(); break;
                case T_MEMOSTRA: comandoEscrita(); break;
                case T_MECAPTURA: comandoLeitura(); break;
                case T_MEREPETE: comandoPara(); break;
                case T_MECOMPARA: comandoSe(); break;
                case T_MECONTINUA: comandoEnquanto(); break;
                default:
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nComando não identificado va aprender a programar pois encontrei: " + lexema); break;
            }
        }

        /*
        * <cmd_enquanto>   ::= MECONTINUA <expcond> ']' <comandos> '['              
        */
        private void comandoEnquanto()
        {
            if (token == T_MECONTINUA)
            {
                compiler.lexicon.buscaProximoToken();
                expressaoCondicional();
                compiler.semantic.regraSemantica(18);
                if (token == T_ABRE_BLOCO)
                {
                    compiler.lexicon.buscaProximoToken();
                    comandos();
                    if (token == T_FECHA_BLOCO)
                    {
                        compiler.lexicon.buscaProximoToken();
                        compiler.semantic.regraSemantica(19);
                    }
                    else
                    {
                        registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nFecha bloco esperado, mas encontrei: " + lexema);
                    }
                }
                else
                {
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nAbre bloco esperado, mas encontrei: " + lexema);
                }
            }
        }

        /* <cmd_se>         ::= 'MECOMPARA' <expcond> '?' 
        *                      ']' <comandos> '['
        *                      <else>
        */
        private void comandoSe()
        {

            if (token == T_MECOMPARA)
            {
                compiler.lexicon.buscaProximoToken();
                expressaoCondicional();
                compiler.semantic.regraSemantica(14);
                if (token == T_ENTAO)
                {
                    compiler.lexicon.buscaProximoToken();
                    if (token == T_ABRE_BLOCO)
                    {
                        compiler.lexicon.buscaProximoToken();
                        comandos();
                        if (token == T_FECHA_BLOCO)
                        {
                            compiler.lexicon.buscaProximoToken();
                            compiler.semantic.regraSemantica(15);
                            senao();
                        }
                        else
                        {
                            registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nFecha bloco esperado, mas encontrei: " + lexema);
                        }
                    }
                    else
                    {
                        registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nAbre bloco esperado, mas encontrei: " + lexema);
                    }

                }
                else
                {
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nMECOMPARA esperado, mas encontrei: " + lexema);
                }
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nMECOMPARA esperado, mas encontrei: " + lexema);
            }
        }

        /*
        * <cmd_else>       ::= ':' ']' <comandos> '[' 
        *                  |   {epsilon} */
        private void senao()
        {
            if (token == T_SENAO)
            {
                compiler.lexicon.buscaProximoToken();
                compiler.semantic.regraSemantica(16);
                if (token == T_ABRE_BLOCO)
                {
                    compiler.lexicon.buscaProximoToken();
                    comandos();
                    if (token == T_FECHA_BLOCO)
                    {
                        compiler.lexicon.buscaProximoToken();
                        compiler.semantic.regraSemantica(17);
                    }
                    else
                    {
                        registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nFecha bloco esperado, mas encontrei: " + lexema);
                    }
                }
                else
                {
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nAbre bloco esperado, mas encontrei: " + lexema);
                }
            }
        }

        //  <cmd_atribuicao> ::= <id> 'VIRA' <exp>
        private void comandoAtribuicao()
        {

            id();
            if (token == T_VIRA)
            {
                compiler.lexicon.buscaProximoToken();
                expressao();
                compiler.semantic.regraSemantica(8);
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nVIRA esperado, mas encontrei: " + lexema);
            }
        }

        // <cmd_escrita> ::= 'MEMOSTRA'')' <exp> '('
        private void comandoEscrita()
        {
            if (token == T_MEMOSTRA)
            {
                compiler.lexicon.buscaProximoToken();
                if (token == T_ABRE_PAR)
                {
                    compiler.lexicon.buscaProximoToken();
                    expressao();
                    if (token == T_FECHA_PAR)
                    {
                        compiler.lexicon.buscaProximoToken();
                        compiler.semantic.regraSemantica(12);
                    }
                    else
                    {
                        registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\n(*)Fecha parentesis esperado, mas encontrei: " + lexema);
                    }
                }
                else
                {
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nAbre parentesis esperado, mas encontrei: " + lexema);
                }
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nMEMOSTRA esperado, mas encontrei: " + lexema);
            }
        }

        // <cmd_leitura> ::= 'MECAPTURA' ')' <id> '('
        private void comandoLeitura()
        {
            if (token == T_MECAPTURA)
            {
                compiler.lexicon.buscaProximoToken();
                if (token == T_ABRE_PAR)
                {
                    compiler.lexicon.buscaProximoToken();
                    id();
                    if (token == T_FECHA_PAR)
                    {
                        compiler.lexicon.buscaProximoToken();
                        compiler.semantic.regraSemantica(9);
                    }
                    else
                    {
                        registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\n(1)Fecha parentesis esperado, mas encontrei: " + lexema);
                    }
                }
                else
                {
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nAbre parentesis esperado, mas encontrei: " + lexema);
                }
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nMECAPTURA esperado, mas encontrei: " + lexema);
            }
        }

        // <cmd_para>       ::= 'MEREPETE' <id> '=>' <exp> '==>' <exp> ']' <comandos> '['
        private void comandoPara()
        {
            if (token == T_MEREPETE)
            {
                compiler.lexicon.buscaProximoToken();
                id();
                if (token == T_REPETE_DE)
                {
                    compiler.lexicon.buscaProximoToken();
                    expressao();
                    if (token == T_REPETE_PARA)
                    {
                        compiler.lexicon.buscaProximoToken();
                        expressao();
                        if (token == T_ABRE_BLOCO)
                        {
                            compiler.lexicon.buscaProximoToken();
                            compiler.semantic.regraSemantica(10);
                            comandos();
                            if (token == T_FECHA_BLOCO)
                            {
                                compiler.lexicon.buscaProximoToken();
                                compiler.semantic.regraSemantica(11);
                            }
                            else
                            {
                                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nFecha bloco esperado, mas encontrei: " + lexema);
                            }
                        }
                        else
                        {
                            registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nAbre bloco esperado, mas encontrei: " + lexema);
                        }
                    }
                    else
                    {
                        registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nRepete para esperado, mas encontrei: " + lexema);
                    }
                }
                else
                {
                    registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nRepete de esperado, mas encontrei: " + lexema);
                }
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nMEREPETE esperado, mas encontrei: " + lexema);
            }
        }

        /*
         * <expcond>        ::= <exp> '<' <exp>
         *                  |   <exp> '<=' <exp>   
         *                  |   <exp> '<>' <exp>   
         *                  |   <exp> '>=' <exp>   
         *                  |   <exp> '>'  <exp>   
         *                  |   <exp> '==' <exp> 
         */
        private void expressaoCondicional()
        {
            expressao();
            if (token == T_MENOR || token == T_MENORIGUAL ||
                 token == T_DIFERENTE || token == T_MAIOR ||
                 token == T_MAIORIGUAL || token == T_IGUAL)
            {
                compiler.tipoDeComparacao = token;
                compiler.lexicon.buscaProximoToken();
                expressao();
                compiler.semantic.regraSemantica(13);
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nExpressao esperada, mas encontrei: " + lexema);
            }
        }

        // <exp> ::= <exp> '+' <termo>
        //       |   <exp> '-' <termo>
        //       |   <termo>
        private void expressao()
        {
            termo();
            if (token == T_SOMA)
            {
                compiler.lexicon.buscaProximoToken();
                expressao();
                compiler.semantic.regraSemantica(6);
            }
            else if (token == T_SUBTRAI)
            {
                compiler.lexicon.buscaProximoToken();
                expressao();
                compiler.semantic.regraSemantica(7);
            }

        }

        private void expressaoAlternativa()
        {
            termo();
            while (token == T_SOMA || token == T_SUBTRAI)
            {
                compiler.lexicon.buscaProximoToken();
                termo();
            }
        }

        // <termo> ::= <termo> '*' <fator>
        //         |   <termo> '/' <fator>
        //         |   <fator>
        private void termo()
        {
            exponenciacao();
            if (token == T_MULTIPLICA)
            {
                compiler.lexicon.buscaProximoToken();
                termo();
                compiler.semantic.regraSemantica(4);
            }
            else if (token == T_DIVIDE)
            {
                compiler.lexicon.buscaProximoToken();
                termo();
                compiler.semantic.regraSemantica(5);
            }

        }

        private void exponenciacao()
        {
            fator();
            if (token == T_ELEVADO)
            {
                compiler.lexicon.buscaProximoToken();
                exponenciacao();
                compiler.semantic.regraSemantica(20);
            }
        }

        // <fator> ::= <numero> | <id>  
        private void fator()
        {
            if (token == T_NUMERO)
            {
                numero();
            }
            else
            {
                id();
            }
        }

        // <id> ::= [A-Z][A-Z,0-9,_]*
        private void id()
        {
            if (token == T_ID)
            {
                compiler.lexicon.buscaProximoToken();
                compiler.semantic.regraSemantica(2);
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nIdentificador esperado, mas encontrei: " + lexema);
            }
        }

        // <numero> ::= [0-9]+
        private void numero()
        {
            if (token == T_NUMERO)
            {
                compiler.lexicon.buscaProximoToken();
                compiler.semantic.regraSemantica(3);
            }
            else
            {
                registraErroSintatico("Erro Sintático na linha: " + linhaAtual + "\nReconhecido ao atingir a coluna: " + colunaAtual + "\nLinha do Erro: <" + linhaFonte + ">\nNUMERO esperado, mas encontrei: " + lexema);
            }
        }

        void registraErroSintatico(string msg)
        {
            if (compiler.compilationState == E_SEM_ERROS)
            {
                compiler.compilationState = E_ERRO_SINTATICO;
                compiler.errorMessage = msg;
            }
        }
    }
}