import {QUESTIONS_FETCH_FAILED, QUESTIONS_FETCH_STARTED, QUESTIONS_FETCH_SUCCESS,} from "../actions/types";

const initialState = {
    data: [],
    error: null,
    isLoading: false
};

export default (state = initialState, {type, payload}) => {
    switch (type) {
        case QUESTIONS_FETCH_STARTED:
            return {
                ...state,
                isLoading: true,
                error: null
            };

        case QUESTIONS_FETCH_FAILED:
            return {
                ...state,
                error: payload,
                isLoading: false
            };

        case QUESTIONS_FETCH_SUCCESS:
            return {
                ...state,
                data: payload,
                error: null,
                isLoading: false
            };

        default:
            return state;
    }
};
