import { Form, Input, Select, DatePicker } from 'antd';

interface PersonalDetailsProps {
  onValuesChange?: (values: any) => void;
}

export default function PersonalDetails({ onValuesChange }: PersonalDetailsProps) {
  return (
    <Form layout="vertical" onValuesChange={onValuesChange}>
      <Form.Item label="Full Name" name="fullName" rules={[{ required: true }]}>
        <Input placeholder="Enter full name" />
      </Form.Item>
      
      <Form.Item label="Date of Birth" name="dateOfBirth" rules={[{ required: true }]}>
        <DatePicker style={{ width: '100%' }} />
      </Form.Item>
      
      <Form.Item label="Employment Status" name="employmentStatus" rules={[{ required: true }]}>
        <Select placeholder="Select employment status">
          <Select.Option value="employed">Employed</Select.Option>
          <Select.Option value="self-employed">Self Employed</Select.Option>
          <Select.Option value="unemployed">Unemployed</Select.Option>
          <Select.Option value="student">Student</Select.Option>
        </Select>
      </Form.Item>
      
      <Form.Item label="Annual Income" name="annualIncome" rules={[{ required: true }]}>
        <Input type="number" placeholder="Enter annual income" />
      </Form.Item>
    </Form>
  );
}