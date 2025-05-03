// @ts-nocheck
import type { Meta, StoryObj } from '@storybook/react';

import KcPage from './KcPage';

const meta = {
   component: KcPage,
} satisfies Meta<typeof KcPage>;

export default meta;

type Story = StoryObj<typeof meta>;

export const Default: Story = {};