import {URL} from "./consts";
import {USERS_FETCH_FAILED, USERS_FETCH_STARTED, USERS_FETCH_SUCCESS} from "./types";
import  axios from "axios";

/**
 * Вытаскивает данные по пользователям
 *
 */
export const fetchUsers = () =>
    (dispatch, getState) => {
        const {isAuth, token} = getState().auth;
        if (!isAuth || !token) return;

        dispatch({
            type: USERS_FETCH_STARTED
        });

        axios.get(`${URL}/api/users`, {
            headers: {'Authorization': "Bearer " + token}
        })
            .then(response => {
                if (response.data)
                    dispatch({
                        type: USERS_FETCH_SUCCESS,
                        payload: response.data
                    });
                else
                    dispatch({
                        type: USERS_FETCH_SUCCESS,
                        payload: []
                    });
            })
            .catch(err => {
                dispatch({
                    type: USERS_FETCH_FAILED,
                    payload: err.response ?
                        err.response.statusText
                        : "failed"
                });
                console.error(err.response ?
                    err.response.statusText
                    : err
                );
            });
    };
