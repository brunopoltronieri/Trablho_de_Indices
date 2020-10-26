# Trabalho Maravilhoso de Organização de Arquivos

Por Júlio e Leopoldo

## Hipótese

Qual o time de futebol brasileiro mais comentado na rede social Twitter?

## Modelo dos arquivos de dados

O arquivo terá registros de tamanho fixo para representar os tweets. O registro será composto dos seguintes campos:

1. id, uint (4 bytes)
1. usuario, UTF-8 string com 50 caracteres (200 bytes)
1. mensagem, UTF-8 string com 280 caracteres (1120 bytes)
1. lista de hashtags, UTF-8 string com 200 caracteres (800 bytes)
1. data, datetime (8 bytes)
1. retweets, uint (4 bytes)

Tamanho total do registro: 2140 bytes contando com o alinhamento