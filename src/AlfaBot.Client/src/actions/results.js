import {URL} from "./consts";
import axios from "axios";
import {RESULTS_FETCH_FAILED, RESULTS_FETCH_STARTED, RESULTS_FETCH_SUCCESS} from "./types";

/**
 * Вытаскивает данные по результатам голосования
 *
 */
export const fetchResults = () =>
    dispatch => {
        dispatch({
            type: RESULTS_FETCH_STARTED
        });

        axios.get(`${URL}/api/Result/top/10`)
            .then(response => {
                dispatch({
                    type: RESULTS_FETCH_SUCCESS,
                    payload: response.data
                });

            })
            .catch(err => {
                dispatch({
                    type: RESULTS_FETCH_FAILED,
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
