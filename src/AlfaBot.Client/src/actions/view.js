import {CHANGE_TAB} from "./types";

export const changeTab = page => dispatch => {
    localStorage.setItem('tab', page);

    dispatch({
        type: CHANGE_TAB,
        payload: page
    });
};

export const loadTab = () => dispatch => {
    let tab = localStorage.getItem('tab');
    if (!tab) return;

    else dispatch({
        type: CHANGE_TAB,
        payload: tab
    });
}

