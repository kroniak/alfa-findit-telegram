import * as action from "./types";
import {URL} from "../consts";
import axios from "axios";

/**
 * Вытаскивает данные по результатам голосования
 *
 */
export const fetchResults = () => {
    return dispatch => {
        dispatch({
            type: action.RESULTS_FETCH_STARTED
        });

        axios.get(`${URL}/api/Result/top/10`)
            .then(response => {
                dispatch({
                    type: action.RESULTS_FETCH_SUCCESS,
                    payload: response.data
                });

            })
            .catch(err => {
                dispatch({
                    type: action.RESULTS_FETCH_FAILED,
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
};
