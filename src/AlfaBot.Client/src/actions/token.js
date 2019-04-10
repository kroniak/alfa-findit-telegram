import * as action from "./types";
import {fetchUsers} from "./users";

export const setToken = token => {
    return dispatch => {
        dispatch({
            type: action.TOKEN_SET,
            payload: token
        });

        dispatch(fetchUsers(token));
    };
};

export const invalidateToken = () => {
    return dispatch => {
        dispatch({
            type: action.TOKEN_INVALIDATE
        });
        if (localStorage) {
            localStorage.removeItem('token');
        }
    };
};

export const saveToken = token => {
    return () => {
        if (localStorage) {
            const tokenLocal = localStorage.getItem('token');
            if (tokenLocal) {
                localStorage.setItem('token', token);
            }
        }
    };
};

export const loadToken = () => {
    return dispatch => {
        if (localStorage) {
            const token = localStorage.getItem('token');
            if (token) {
                dispatch(setToken(token));
            }
        }
    };
};
