# secomp-jukebox

Jukebox colaborativa desenvolvida para a 8ª Semana da Computação do Inatel. 5 músicas aleatorias são sorteadas para votação, e a mais votada é a próxima a ser tocada. Para votar em uma música, envie um tweet contendo "#JukeboxInatel <número da música>". Os tweets aparecem ao vivo no ticker na parte de baixo da tela.

Este código está sendo lançado exatamente do jeito em que foi criado, ou seja, não nos responsabilizamos caso ele apague seu HD, mate seu gato ou comece a Terceira Guerra Mundial :D

![Screenshot](http://i.imgur.com/FTvUcqG.png "Screenshot")

## Notas

Para iniciar a obtenção dos tweets, clique no ícone de 'Atualizar' no canto superior esquerdo da tela.

A Jukebox utiliza a API [Stream](https://dev.twitter.com/streaming/overview "Stream") do Twitter para a obtenção dos votos em tempo real. As capas dos álbuns são obtidas utilizando a [Spotify Web API](https://developer.spotify.com/web-api/ "Spotify Web API"). O software por si só não reproduz nenhum áudio; ele utiliza o Spotify para a reprodução das músicas.

O arquivo TrackList.txt, na pasta Resources do aplicativo, contém a lista de músicas disponíveis para votação, no formato:

Música|Artista|URI do Spotify

Para adicionar/remover mais músicas no aplicativo, basta fechar a jukebox, editar o arquivo e abri-la novamente.

O software foi desenvolvido para telas com resolução de 1920x1080, sendo renderizado incorretamente em telas com resolução menor.

## Dependências

1. [Visual Studio](https://www.visualstudio.com/ "Visual Studio") 2013 Express for Desktop, Community ou Professional.
2. [Spotify](https://spotify.com/ "Spotify"), de preferência com uma conta Spotify Premium
3. [Tweetinvi](https://tweetinvi.codeplex.com/ "Tweetinvi") (inclusa com o projeto) e uma [API/Consumer Key](https://apps.twitter.com/ "API/Consumer Key") válida.
4. [SpotifyAPI-NET](https://github.com/JohnnyCrazy/SpotifyAPI-NET "SpotifyAPI-NET") (inclusa com o projeto)
