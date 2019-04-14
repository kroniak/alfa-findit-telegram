import * as action from "./types";
import {URL} from "../consts";
import axios from "axios";
import {invalidateToken, saveToken} from "./token";

/**
 * Вытаскивает данные по пользователям
 *
 */
export const fetchUsers = token => {
    return dispatch => {
        dispatch({
            type: action.USERS_FETCH_STARTED
        });

        if (token !== null) {
            axios.get(`${URL}/api/users?secretkey=${token}`)
                .then(response => {
                    dispatch(saveToken(token));
                    if (response.data)
                        dispatch({
                            type: action.USERS_FETCH_SUCCESS,
                            payload: response.data
                        });
                    else
                        dispatch({
                            type: action.USERS_FETCH_SUCCESS,
                            payload: []
                        });
                })
                .catch(err => {
                    dispatch({
                        type: action.USERS_FETCH_FAILED,
                        payload: err.response ?
                            err.response.statusText
                            : "failed"
                    });
                    dispatch(invalidateToken());

                    console.error(err.response ?
                        err.response.statusText
                        : err
                    );
                });
        } else {
            dispatch({
                type: action.USERS_FETCH_FAILED,
                payload: "token is null"
            });
        }
    };
};
