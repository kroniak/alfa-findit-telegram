import {URL} from "./consts";
import axios from "axios";
import {QUESTIONS_FETCH_FAILED, QUESTIONS_FETCH_STARTED, QUESTIONS_FETCH_SUCCESS} from "./types";

export const fetchQuestions = () =>
    (dispatch, getState) => {
        const {isAuth, token} = getState().auth;
        if (!isAuth || !token) return;

        dispatch({
            type: QUESTIONS_FETCH_STARTED
        });

        axios.get(`${URL}/api/questions/`, {
            headers: {'Authorization': "Bearer " + token}
        })
            .then(response => {
                dispatch({
                    type: QUESTIONS_FETCH_SUCCESS,
                    payload: response.data
                });

            })
            .catch(err => {
                dispatch({
                    type: QUESTIONS_FETCH_FAILED,
                    payload: err.response ?
                        err.response.data.message
                            ? err.response.data.message
                            : err.response.data
                        : "failed"
                });
                console.error(err.response ?
                    err.response.data.message
                        ? err.response.data.message
                        : err.response.data
                    : err
                );
            });
    };
