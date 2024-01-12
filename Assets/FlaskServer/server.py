from flask import Flask, request, jsonify
from flask_cors import CORS
import mysql.connector

app = Flask(__name__)
CORS(app)

# MySQL 연결 설정
db_config = {
    'user': 'root',
    'password': '',
    'host': '127.0.0.1',
    'database': 'game_database',
}

# 모든 데이터는 json형태로 클라이언트에게 전달

# 로그인 버튼을 눌렀을 때 서버 처리 / userid 정보 필요
# 현재 로그인 시 서버에 요청하지 않고 닉네임 생성 여부를 확인하므로 아래 코드를 사용하지 않고 있음.
@app.route('/log_in', methods=['POST'])
def check_login():
    try:
        print("login check..")
        data = request.get_json() 
        userid = data['userid']
        
        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()
        
        query = f"SELECT nickname FROM users WHERE userid = '{userid}'"
        cursor.execute(query)
        result = cursor.fetchall()
        
        # 닉네임을 설정한 유저일 경우 - true, 닉네임 정보 리턴
        if result:
            nickname = result[0]
            return jsonify(status=True, message=nickname)
        # 첫 번째 로그인일 경우 - false 리턴
        else:
            return jsonify(status=False, message="Sign up required.")
        
    except Exception as e:
        return jsonify(error=str(e)), 500
    
# 닉네임 생성 버튼을 눌렀을 때 서버 처리 / userid, nickname 정보 필요
@app.route('/sign_up', methods=['POST'])
def create_nickname():
    try:
        print("sign up...")
        data = request.get_json()
        userid = data['userid']
        nickname = data['nickname']

        # MySQL 연결
        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()

        # MySQL에서 사용자 검색
        query = f"SELECT * FROM users WHERE nickname = '{nickname}'"
        cursor.execute(query)
        result = cursor.fetchall()

        # 중복된 닉네임일 경우 - false 리턴
        if result:
            return jsonify(status=False, message="Duplicated nickname.")
        # 중복되지 않은 닉네임일 경우 - true와 함께 생성된 닉네임 리턴
        else:
            query = f"INSERT INTO users (userid, nickname, point) VALUES ('{userid}', '{nickname}', {1000})"
            cursor.execute(query)
            db.commit()
            return jsonify(status=True, message=nickname)

    except Exception as e:
        return jsonify(error=str(e)), 500
    
# 전체 방 목록을 조회할 때 서버 처리
@app.route('/room_list', methods=['POST'])
def room_list():
    try:
        print("room list...")

        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()

        # MySQL 데이터베이스에 들어 있는 모든 방 이름 조회
        query = "SELECT roomname, roompw FROM rooms"
        cursor.execute(query)
        
        # 결과를 딕셔너리 형태의 리스트로 저장
        result = [{"roomname": row[0], "roompw": row[1]} for row in cursor.fetchall()]

        return jsonify(result=result)
        
    except Exception as e:
        return jsonify(error=str(e)), 500
    
# 검색한 방 이름에 해당하는 방들을 조회할 때 서버 처리 / searchname 정보 필요
@app.route('/search_room', methods=['POST'])
def search_room():
    try:
        print("search room...")
        data = request.get_json()
        searchname = data['searchname']
        
        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()
        
        # MySQL에서 검색 결과에 해당하는 방 이름 조회
        query = f"SELECT roomname, roompw FROM rooms WHERE roomname LIKE '%{searchname}%'"
        cursor.execute(query)
        
        # 결과를 딕셔너리 형태의 리스트로 저장
        result = [{"roomname": row[0], "roompw": row[1]} for row in cursor.fetchall()]

        return jsonify(result=result)
            
    except Exception as e:
        return jsonify(error=str(e)), 500
    
# 방을 생성했을 때 서버 처리 / roomname, roompw 정보 필요
@app.route('/create_room', methods=['POST'])
def create_room():
    try:
        print("create room...")
        data = request.get_json()
        roomname = data['roomname']
        roompw = data['roompw']
        
        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()
        
        # MySQL에서 같은 이름의 방이 존재하는지 검색
        query = f"SELECT * FROM rooms WHERE roomname = '{roomname}'"
        cursor.execute(query)
        result = cursor.fetchall()
        
        # 중복된 이름의 방이 존재할 경우 - false 리턴
        if result:
            return jsonify(status=False, message="Duplicated room name.")
        # 중복되지 않은 방일 경우 - 데이터베이스에 방 정보를 넣은 뒤 true 리턴
        else:
            query = f"INSERT INTO rooms (roomname, roompw) VALUES ('{roomname}', '{roompw}')"
            cursor.execute(query)
            db.commit()
            return jsonify(status=True, message="Room created.")
            
    except Exception as e:
        return jsonify(error=str(e)), 500    
    
# 방을 삭제할 때 서버 처리 / roomname 정보 필요
@app.route('/delete_room', methods=['POST'])
def delete_room():
    try:
        print("delete room...")
        data = request.get_json()
        roomname = data['roomname']
        
        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()
        
        # MySQL에서 같은 이름을 가진 방 삭제
        query = f"DELETE FROM rooms WHERE roomname = '{roomname}'"
        cursor.execute(query)
        db.commit()
        
        deleted_rows = cursor.rowcount
        
        # 성공적으로 방을 삭제했을 때 true 리턴
        if deleted_rows > 0:
            return jsonify(status=True, message=f"room '{roomname}' deleted")
        else:
            return jsonify(status=False, message=f"error: cannot delete room")
        
    except Exception as e:
        return jsonify(error=str(e)), 500
    
# 승리 및 패배 후 점수 변동에 대한 서버 처리 / nickname, result 정보 필요
@app.route('/on_result', methods=['POST'])
def on_result():
    try:
        print("update point...")
        data = request.get_json()
        nickname = data['nickname']
        result = data['result']
        
        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()
        
        # MySQL에서 플레이어의 결과에 따라 point 변경
        # 승리
        if (result == 'true'):
            query = f"UPDATE users SET point = point + 10 WHERE nickname = '{nickname}'"
        # 패배
        else:
            query = f"UPDATE users SET point = point - 10 WHERE nickname = '{nickname}'"
        cursor.execute(query)
        db.commit()
        
        updated_rows = cursor.rowcount
        
        # 성공적으로 점수를 업데이트했을 때 true 리턴
        if updated_rows > 0:
            return jsonify(status=True, message=f"successfullty updated")
        else:
            return jsonify(status=False, message=f"error: cannot update point")
        
    except Exception as e:
        return jsonify(error=str(e)), 500

@app.route('/show_ranking', methods=['POST'])
def show_ranking():
    try:
        print("show ranking...")
        db = mysql.connector.connect(**db_config)
        cursor = db.cursor()
        
        # MySQL에서 플레이어의 닉네임과 point 정보 가져오기
        query = f"SELECT nickname, point From users"
        cursor.execute(query)
        
        # 결과를 딕셔너리 형태의 리스트로 저장
        result = [{"nickname": row[0], "point": row[1]} for row in cursor.fetchall()]
        # 결과를 점수가 높은 순으로 정렬
        sorted_result = sorted(result, key=lambda x: x["point"], reverse=True)
        return jsonify(result=sorted_result)
        
    except Exception as e:
        return jsonify(error=str(e)), 500

if __name__ == '__main__':
    # 포트를 80으로 설정 (방화벽에서 허용한 포트 번호임)
    app.run(host='0.0.0.0', port=80, debug=False, threaded=True)
