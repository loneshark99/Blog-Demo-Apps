import requests;

def SendRequests():
    for x in range(1,100):
        resp = requests.get("http://localhost:8080/")
        print(resp.content)


if __name__ == '__main__':
    SendRequests()