import sys
import requests
from requests.exceptions import ConnectTimeout, ReadTimeout, ConnectionError, RequestException


API_URL = "http://192.168.200.37/RestAnaplan-Test/CalendarHoliday"  # endpoint
QUERY_PARAMS = {
     "KeyAuth": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
}
CONNECT_TIMEOUT_SECS = 5.0  
READ_TIMEOUT_SECS = 15.0

def main() -> int:
    try:
        resp = requests.get(
            API_URL,
            params=QUERY_PARAMS,
            headers={"Accept": "application/json"},
            timeout=(CONNECT_TIMEOUT_SECS, READ_TIMEOUT_SECS),
        )

        # Success
        if 200 <= resp.status_code < 300:
            if resp.status_code == 204:
                print(f"[SUCCESS] 204 from {API_URL}")
            else:
                print(f"[SUCCESS] {resp.status_code} from {API_URL}")
             
                try:
                    print("Response JSON:", resp.json())
                except ValueError:
                    if resp.text:
                        print("Response Text:", resp.text)
            return 0

        # failed
        print(f"[FAILED] HTTP {resp.status_code} from {API_URL}")
        if resp.text:
            print("Response Text:", resp.text)
        return 1

    except ConnectTimeout:
        print(f"[FAILED] Could not connect to {API_URL} (connect timeout).")
        return 2

    except ReadTimeout:
        print(f"[FAILED] Timed out waiting for response from {API_URL}.")
        return 3

    except ConnectionError as e:
        print(f"[FAILED] Network/connection error: {e}")
        return 4

    except RequestException as e:
        print(f"[FAILED] Unexpected error: {e}")
        return 5


if __name__ == "__main__":
    sys.exit(main())