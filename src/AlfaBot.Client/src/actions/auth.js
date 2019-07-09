import {AUTH_URL, VERIFY_URL} from "./consts";
import {
    USER_LOGIN_FAILED,
    USER_LOGIN_STARTED,
    USER_LOGIN_SUCCESS,
    USER_LOGOUT,
    USER_TOKEN_VERIFY_COMPLETE, USER_TOKEN_SET,
    USER_TOKEN_VERIFY_START
} from "./types";

import axios from "axios";

export const verifyToken = () => {
    return dispatch => {
        let token = localStorage.getItem('token');
        if (!token) return;

        dispatch({
            type: USER_TOKEN_VERIFY_START
        });

        dispatch({
            type: USER_TOKEN_SET,
            payload: {
                token: token
            }
        });

        axios.get(VERIFY_URL, {
            headers: {
                authorization: 'Bearer ' + token
            }
        })
            .then(response => {
                dispatch({
                    type: USER_LOGIN_SUCCESS,
                    payload: {
                        userName: response.data.userName,
                        token: token
                    }
                });
            })
            .catch(err => {
                console.error(err);
                if (err.response && err.response.status === 401) {
                    dispatch(logoutUser());
                }
            })
            .finally(() => {
                dispatch({
                    type: USER_TOKEN_VERIFY_COMPLETE
                });
            });
    };
};

export const logUser = (userName, password) => dispatch => {
    const data = {
        userName,
        password
    };

    dispatch({
        type: USER_LOGIN_STARTED
    });

    axios
        .post(`${AUTH_URL}`, data)
        .then(response => {
            if (response.status === 200) {
                if (response.data.token) {
                    localStorage.setItem('token', response.data.token);
                    dispatch({
                        type: USER_LOGIN_SUCCESS,
                        payload: {
                            token: response.data.token,
                            userName: userName
                        }
                    });
                }
            } else
                dispatch({
                    type: USER_LOGIN_FAILED,
                    payload: "Что то пошло не так..."
                });
        })
        .catch(err => {
            if (err.response) {
                console.error(
                    err.response.data.message
                        ? err.response.data.message
                        : err.response.data
                );
                dispatch({
                    type: USER_LOGIN_FAILED,
                    payload: err.response.data.message
                        ? err.response.data.message
                        : err.response.data
                });
            } else {
                dispatch({
                    type: USER_LOGIN_FAILED,
                    payload: "failed login"
                });
            }
        });
};

export const logoutUser = () => dispatch => {
    localStorage.removeItem('token');
    dispatch({
        type: USER_LOGOUT,
    });
};
