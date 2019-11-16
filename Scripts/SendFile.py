import requests

data = {
    'return': 'artist',
    'api_token': 'e6c4599bd9cbf07eac4c17ab611cb02d'
}
result = requests.post('https://api.audd.io/', data=data, files={'file': open('C:\\Users\\user\Documents\\Visual Studio 2019\\Projects\\MusicRecogniser\\Output\\myaudio.mp3', 'rb')})
print(result.text)
