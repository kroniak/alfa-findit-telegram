import React, {Component} from 'react';
import {connect} from "react-redux";
import styled from "@emotion/styled";

import Heading from 'arui-feather/heading/';
import Button from "arui-feather/button/";
import {SpinCustom as Spin} from "../misc/misc";
import {fetchQuestions} from "../../actions/questions";
import Plate from "arui-feather/plate";
import Notification from "../misc/notification";

const QuestionGrid = styled.div`
  margin-top: 20px;
  display: grid;
  grid-template-columns: 1fr auto;
`;

const renderQuestion = ({id, isPicture, message}) => (<Plate
    hasCloser={false}
    key={id}>
    {!isPicture && <Heading size='m'>
        {message}
    </Heading>}
</Plate>);

const renderQuestionsList = questions => {
    if (questions && questions.length) return questions.map(q => renderQuestion(q));
    else return null;
};

class QuestionsContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isLoading: false
        }
    }

    componentWillReceiveProps(nextProps, nextContext) {
        if (nextProps.isLoading) {
            this.setState({
                isLoading: true
            });
            setTimeout(() => {
                this.setState({
                    isLoading: false
                });
            }, 500);
        }
    }

    componentDidMount() {
        this.props.fetchQuestions();
    }

    render() {
        const {questions, fetchQuestions, error} = this.props;
        const {isLoading} = this.state;
        return (
            <div>
                <Notification
                    title="Ошибка закгрузки"
                    message="Что то пошло не так! Возможно нет связи с сервером"
                    error={error}
                />
                <Heading size='m'>
                    Список вопросов
                </Heading>
                <Button disabled={isLoading}
                        view='extra'
                        size='m'
                        onClick={fetchQuestions}>
                    Обновить
                </Button>
                <Spin
                    size='m'
                    visible={isLoading}
                />
                <QuestionGrid>
                    {renderQuestionsList(questions)}
                </QuestionGrid>
            </div>
        );
    }
}

const mapStateToProps = state => ({
    questions: state.questions.data,
    error: state.questions.error,
    isLoading: state.questions.isLoading
});

const mapDispatchToProps = dispatch => ({
    fetchQuestions: () => dispatch(fetchQuestions()),
});

export default connect(mapStateToProps, mapDispatchToProps)(QuestionsContainer);